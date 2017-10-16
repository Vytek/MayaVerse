using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevConsole.Internal;
using StringBuilder = System.Text.StringBuilder;

namespace DevConsole{
	[System.Serializable]
	public class Console : MonoBehaviour,ISerializationCallbackReceiver {
		
		//=============================
		//VARS
		//=============================
		[SerializeField]
		bool dontDestroyOnLoad;							//Sets whether it destroys on load or not
		[SerializeField]
		KeyCode consoleKey = KeyCode.Backslash;			//Key to open/close Console
		[RangeAttribute(8, 20)]
		public int fontSize;

		const int TEXT_AREA_OFFSET = 7;					//Margin for the text at the bottom of the console
		bool helpEnabled = true;						//Is the help for autocompletion on?
		int numHelpCommandsToShow = 5;					//Max candidates to show on autocompletion
		float helpWindowMinWidth = 200;					//The minimum width of the autocompletion window
		const int WARNING_THRESHOLD = 15000;			//Number of characters in consoleText at which a warning appears
		const int DANGER_THRESHOLD = 16000;				//Number of characters in consoleText at which a danger appears	
		const int AUTOCLEAR_THRESHOLD = 18000;          //At this many characters, a DC_CLEAR will be done automatically

		List<CommandBase> candidates = new List<CommandBase>();	//Commands that match existing text
		int selectedCandidate = 0;						//Index of candidate selected
		List<string> history = new List<string>();		//A history of texts sent into the console
		int selectedHistory = 0;						//Current index in the history
		List<KeyValuePair<string, string>> buffer = 
			new List<KeyValuePair<string, string>>();   //Messages buffer. Stored in order to print them on the next OnGUI update

        CommandsManager _manager;
        CommandsManager manager {
            get {
                if (_manager == null) {
                    _manager = new CommandsManager();
                    _manager.Load();
                }
                return _manager;
            }
        }


		static Console _singleton;						//Singleton
		static Console Singleton{						
			get{
				if (_singleton==null)
					_singleton = GameObject.FindObjectOfType<Console>();
				return _singleton;
			}
		}					

		bool opening;									//Can write already?
		bool closed = true;								//Is the Console closed?
		bool showHelp = true;							//Should Help Window show?
		bool inHistory = false;							//Are we browsing the history?
		bool showTimeStamp = false;						//Should time stamps be shown?
		
		float numLinesThreshold;						//Max numes allowed in the console
		float maxConsoleHeight; 						//Screen.height/3
		
		float currentConsoleHeight;						//Current Y position in pixels
		Vector2 consoleScroll = Vector2.zero;			
		Vector2 helpWindowScroll = Vector2.zero;

		[HideInInspector]
		[SerializeField] 
		string serializedConsoleText = string.Empty;
		StringBuilder consoleText = new StringBuilder();//Text in the whole console log
		string inputText = string.Empty;				//Text in the input TextField
		string lastText = string.Empty;					//Used for history mostly
		int numLines;									//Number of '\n' in consoleText
		float lineHeight;
		[SerializeField]
		Settings extraSettings;
		public static bool verbose{get{return Singleton.extraSettings.logVerbose;}set{Singleton.extraSettings.logVerbose = value;}}
		public static bool isOpen{get{return !Singleton.closed;}}
		
		//=============================
		//INITIALIZATION
		//=============================
		void Awake(){
			if (Singleton != this){
				Debug.LogWarning("There can only be one Console per project");
				Destroy(this); 
				return;
			}
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
			if (extraSettings.showDebugLog)
				Application.logMessageReceived+=LogCallback;
		}

        public void OnBeforeSerialize() {
            serializedConsoleText = consoleText.ToString();
        }

		public void OnAfterDeserialize(){
			consoleText.Append(serializedConsoleText);
		}

		//=============================
		//GUI
		//=============================
		void OnGUI(){
			GUISkin oldSkin = GUI.skin;
			if (extraSettings.skin != null)
				GUI.skin = extraSettings.skin;

			Event current = Event.current;
			GUI.skin.textArea.richText = true;
			if (extraSettings.font != null)
				GUI.skin.font = extraSettings.font;

			GUI.skin.textArea.fontSize = GUI.skin.textField.fontSize = fontSize;

            //Open/Close Console
            if (current.type == EventType.KeyDown && current.keyCode == consoleKey && consoleKey != KeyCode.None){
				GUIUtility.keyboardControl = 0;
				StartCoroutine(FadeInOut(closed));
			}
			
			lineHeight = GUI.skin.textArea.lineHeight;
			//Local declarations
			bool moving = !((currentConsoleHeight == maxConsoleHeight) || (currentConsoleHeight == 0));
			float height = lineHeight*numLines;
			float scrollHeight = height>currentConsoleHeight?height:currentConsoleHeight;

			if (!closed){
				//Treat buffer
				for (int i = 0; i< buffer.Count; i++)
					BasePrintOnGUI(buffer[i].Key, buffer[i].Value);
				buffer.Clear();

				if (!moving)
					GUI.FocusControl("TextField");
				//KEYS
				if (current.type == EventType.keyDown){
					if (!string.IsNullOrEmpty(inputText)){
						switch(current.keyCode){
						case KeyCode.Return:
                            if (candidates.Count == 0)
                                PrintInput(inputText);
                            else
                                SelectCurrentCandidate();
                            break;
						case KeyCode.Tab:
							if (candidates.Count !=0)
                                SelectCurrentCandidate();
							break;
						case KeyCode.Escape:
							showHelp = false;
							candidates.Clear();
							break;
						case KeyCode.F1:
							showHelp = true;
							break;
						}
					}
					switch(current.keyCode){
					case KeyCode.UpArrow:
						if ((inHistory || inputText == string.Empty) && history.Count != 0){
							selectedHistory = Mathf.Clamp(selectedHistory+(inHistory?1:0), 0, history.Count-1);
							inputText = history[selectedHistory];
							showHelp = false;
							inHistory = true;
							lastText = inputText;
						}
						else if (inputText != string.Empty && !inHistory){
							selectedCandidate = Mathf.Clamp(--selectedCandidate,0, candidates.Count-1);
							if (selectedCandidate*lineHeight <= helpWindowScroll.y ||
							    selectedCandidate*lineHeight > helpWindowScroll.y + lineHeight*(numHelpCommandsToShow-1))	 
								helpWindowScroll = new Vector2(0,selectedCandidate*lineHeight-1*lineHeight);
						}
						SetCursorPos(inputText, inputText.Length);
						break;
					case KeyCode.DownArrow:
						if ((inHistory || inputText == string.Empty) && history.Count != 0){
							selectedHistory = Mathf.Clamp(selectedHistory-(inHistory?1:0), 0, history.Count-1);
							inputText = history[selectedHistory];
							showHelp = false;
							inHistory = true;
							lastText = inputText;
						}
						else if (inputText != string.Empty && !inHistory){
							selectedCandidate = Mathf.Clamp(++selectedCandidate,0, candidates.Count-1);
							if (selectedCandidate*lineHeight > helpWindowScroll.y + lineHeight*(numHelpCommandsToShow-2) ||
							    selectedCandidate*lineHeight < helpWindowScroll.y)
								helpWindowScroll = new Vector2(0, selectedCandidate*lineHeight-((numHelpCommandsToShow-2)*lineHeight));
						}
						SetCursorPos(inputText, inputText.Length);
						break;
					}
				}
				
				if (lastText != inputText){
					inHistory = false;
					lastText = string.Empty;
				}
				//CONSOLE PAINTING
				GUI.Box(new Rect(0,0,Screen.width, currentConsoleHeight), new GUIContent());
				GUI.SetNextControlName("TextField");
				GUI.enabled = !opening;
				inputText = GUI.TextField(new Rect(0, currentConsoleHeight + 0, Screen.width, 25), inputText);
				GUI.enabled = true;
				GUI.skin.textArea.normal.background = null;
				GUI.skin.textArea.hover.background = null;
				consoleScroll = GUI.BeginScrollView(new Rect(0,0,Screen.width, currentConsoleHeight),consoleScroll,
				                                    new Rect(0, 0, Screen.width-20, scrollHeight));
				GUI.TextArea(new Rect(0, -5+currentConsoleHeight-0-(numLines==0?-0+lineHeight:height)+
				                      (numLines>=numLinesThreshold-1?lineHeight*(numLines-numLinesThreshold):0), 
										Screen.width,TEXT_AREA_OFFSET+(numLines==0?lineHeight:height)), consoleText.ToString());
				GUI.EndScrollView();
				if (inputText == string.Empty)
					showHelp= true;
			}

			//HELP WINDOW
			if (showHelp && helpEnabled && inputText.Trim ()!=string.Empty){
				ShowHelp();
				if (candidates.Count != 0){
					GUI.skin.textArea.normal.background = GUI.skin.textField.normal.background;
					GUI.skin.textArea.hover.background = GUI.skin.textField.hover.background;

					StringBuilder help = new StringBuilder();
                    float helpWindowWidth = this.helpWindowMinWidth;
                    for (int i = 0; i < candidates.Count; i++) {
                        string signature = candidates[selectedCandidate] == candidates[i] ? "<color=yellow>" + candidates[i].fullSignature +
                            "</color>" : candidates[i].fullSignature;
                        float widthNeeded = GUI.skin.textArea.CalcSize(new GUIContent(signature)).x;
                        helpWindowWidth = Mathf.Max(helpWindowWidth, widthNeeded);
                        help.Append(signature + '\n');
                    }

					if (candidates.Count > numHelpCommandsToShow){
						helpWindowScroll = GUI.BeginScrollView(new Rect(0,currentConsoleHeight-numHelpCommandsToShow*lineHeight-TEXT_AREA_OFFSET,
	                                                helpWindowWidth,5+lineHeight*numHelpCommandsToShow), helpWindowScroll,
						                                       new Rect(0, 0,helpWindowWidth-20,TEXT_AREA_OFFSET + candidates.Count*lineHeight));
						GUI.TextArea(new Rect(0, 0, helpWindowWidth, TEXT_AREA_OFFSET + candidates.Count*lineHeight),help.ToString());
						GUI.EndScrollView();
					}
					else
						GUI.TextArea(new Rect(0,currentConsoleHeight-TEXT_AREA_OFFSET-
					                      (candidates.Count > numHelpCommandsToShow?numHelpCommandsToShow*lineHeight:
                               				lineHeight*candidates.Count),helpWindowWidth,
			                     		 (candidates.Count > numHelpCommandsToShow?numHelpCommandsToShow*lineHeight:
					 						lineHeight*candidates.Count)+TEXT_AREA_OFFSET),help.ToString());
				}
			}
			GUI.skin = oldSkin;	
		}

        void SelectCurrentCandidate() {
            inputText = candidates[selectedCandidate].fullName;
            showHelp = false;
            candidates.Clear();
            SetCursorPos(inputText, inputText.Length);
        }

        //=============================
        //OTHERS
        //=============================
        public static void Open(){
			if (isOpen)
				return;
			GUIUtility.keyboardControl = 0;
			Singleton.StartCoroutine(Singleton.FadeInOut(true));
		}

		public static void Close(){
			if (!isOpen)
				return;
			GUIUtility.keyboardControl = 0;
			Singleton.StartCoroutine(Singleton.FadeInOut(false));
		}

		IEnumerator FadeInOut(bool open){
			if (opening)
				yield break;
			opening = true;
			maxConsoleHeight = Screen.height/3;
			numLinesThreshold = maxConsoleHeight/lineHeight;
			closed = false;
			float duration = extraSettings.curve[extraSettings.curve.length-1].time;
			float time = 0;
			do{
				currentConsoleHeight = maxConsoleHeight*extraSettings.curve.Evaluate(open?time:duration-time);
				yield return null;
				time=Mathf.Clamp(time+Time.unscaledDeltaTime,0,duration);
			}while (time<duration);
			currentConsoleHeight = maxConsoleHeight*extraSettings.curve.Evaluate(open?time:duration-time);
			closed = !open;
			if (closed)
				inputText =string.Empty;
			opening = false;
		}
		
		void ShowHelp(){
            CommandBase[] commands = manager.GetCommands();
			CommandBase aux = null;
			if (candidates.Count != 0 && selectedCandidate >= 0 && candidates.Count > selectedCandidate)
				aux = candidates[selectedCandidate];
			candidates.Clear();
			for (int i = 0; i < commands.Length; i++){
				if(commands[i].fullName.ToUpper().StartsWith(inputText.ToUpper()))
					candidates.Add(commands[i]);
			}
			if (aux == null){
				selectedCandidate = 0;
				return;
			} 
			for(int i = 0; i < candidates.Count; i++){
				if (candidates[i] == aux){
					selectedCandidate = i;
					return;
				}
			}
			selectedCandidate = 0;
		}

		#region Tools
		void SetCursorPos(string text, int pos){
			TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor),GUIUtility.keyboardControl);
			#if UNITY_5_2
			te.content = new GUIContent(text);
			te.cursorIndex = pos;
			te.selectIndex = pos;
			#elif UNITY_5_3_OR_NEWER
			te.text = text;
			te.cursorIndex = pos;
			te.selectIndex = pos;
			#else
			te.selectPos = pos; 
			te.pos= pos;
			#endif
			GUIUtility.ExitGUI();
		}

		public static string ColorToHex(Color color){
			string hex ="0123456789ABCDEF";
			int r = (int)(color.r*255);
			int g = (int)(color.g*255);
			int b = (int)(color.b*255);

			return	hex[(int)(Mathf.Floor(r/16))].ToString()+hex[(int)(Mathf.Round(r%16))].ToString()+
				hex[(int)(Mathf.Floor(g/16))].ToString()+hex[(int)(Mathf.Round(g%16))].ToString()+
				hex[(int)(Mathf.Floor(b/16))].ToString()+hex[(int)(Mathf.Round(b%16))].ToString();
		}

		#endregion
		//=============================
		//PRINTS
		//=============================
		#region Logs
		/// <summary>
		/// Logs a white text to the console.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public static void Log(string text){
			Singleton.BasePrint(text);
		}
		public static void Log(object obj){
			Log (obj.ToString());
		}
		/// <summary>
		/// Logs a ligh blue text to the console.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public static void LogInfo(string text){
			Singleton.BasePrint(text, Color.cyan);
		}
		public static void LogInfo(object obj){
			LogInfo (obj.ToString());
		}
		/// <summary>
		/// Logs a yellow text to the console.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public static void LogWarning(string text){
			Singleton.BasePrint(text, Color.yellow);
		}
		public static void LogWarning(object obj){
			LogWarning (obj.ToString());
		}
		/// <summary>
		/// Logs a red text to the console.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public static void LogError(string text){
			Singleton.BasePrint(text, Color.red);
		}
		public static void LogError(object obj){
			LogError (obj.ToString());
		}
		/// <summary>
		/// Logs a text to the console with the specified color.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		/// <param name="color">Color provided in HTML format.</param>
		public static void Log(string text, string color){
			Singleton.BasePrint(text, color);
		}
		public static void Log(object obj, string color){
			Log(obj.ToString(), color);
		}
		/// <summary>
		/// Logs a text to the console with the specified color.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		/// <param name="color">Color to be used.</param>
		public static void Log(string text, Color color){
			Singleton.BasePrint(text,color);
		}
		public static void Log(object obj, Color color){
			Log(obj.ToString(), color);
		}
		#endregion
		#region Prints
		void BasePrint(string text){
			BasePrint(text, ColorToHex(Color.white));
		}

		void BasePrint(string text, Color color){
			BasePrint(text, ColorToHex(color));	
		}

		void BasePrint(string text, string color){
			buffer.Add(new KeyValuePair<string, string>(text, color));
		}

		//In case print is called from another thread, the action is cached in a buffer and processed in the next OnGUI update
		void BasePrintOnGUI(string text, string color){
			text = "> " + text;
			int numLineJumps = 1;
			string time = (showTimeStamp?"["+System.DateTime.Now.ToShortTimeString()+"]  ":string.Empty);
			StringBuilder lastLine = new StringBuilder(time);
			for (int i = 0; i < text.Length; i++){
				if (text[i] == '\n'){
					numLineJumps++;
					lastLine = new StringBuilder(time);
				}
				else
					lastLine.Append(text[i]);
				if (GUI.skin.textArea.CalcSize(new GUIContent(lastLine.ToString())).x>Screen.width-20){
					text = text.Insert(i, "\n");
					i--;
				}
			}
			text += '\n';
			numLines+= numLineJumps;
			if (numLines >=numLinesThreshold-1)
				consoleScroll = new Vector2(0,consoleScroll.y+int.MaxValue);
			AddText(text, color);
			if (consoleText.Length>=AUTOCLEAR_THRESHOLD){
				Clear();
				AddText("Buffer cleared automatically\n", ColorToHex(Color.yellow));
			}
			else if (consoleText.Length>=DANGER_THRESHOLD)
				AddText("Buffer size too large. You should clear the console\n", ColorToHex(Color.red));
			else if (consoleText.Length>=WARNING_THRESHOLD)
				AddText("Buffer size too large. You should clear the console\n", ColorToHex(Color.yellow));
		}

		void AddText(string text, string color){
			consoleText.Append(string.Format("{0}<color=#{1}>{2}</color>",showTimeStamp?"["+System.DateTime.Now.ToShortTimeString()+"]  ":string.Empty, color, text));
		}
		
		void PrintInput(string input){
			inputText = string.Empty;
			if ((history.Count == 0 || history[0] != input) && input.Trim() != string.Empty)
				history.Insert(0,input);
			selectedHistory = 0;
			BasePrint(input);
			ExecuteCommandInternal(input);
		}

		void LogCallback(string log, string stackTrace, LogType type){
			Color color;
			if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
				color = Color.red;
			else if (type == LogType.Warning)
				color = Color.yellow;
			else// if (type == LogType.Log)
				color = Color.cyan;
			BasePrint(log, color);
			BasePrint(stackTrace, color);

			int length =(int) GUI.skin.textArea.CalcSize(new GUIContent(log)).x;
			while (length>= Screen.width){
				length-=Screen.width;
				numLines++;
			}
			numLines++;
		}
		#endregion
		//===========================
		//COMMANDS
		//===========================
		#region Manage Commands
		public static void ExecuteCommand(string command){
			Singleton.ExecuteCommandInternal(command);
		}

		public static void ExecuteCommand(string command, string args){
			Singleton.ExecuteCommandInternal(command+" "+args);
		}

		void ExecuteCommandInternal(string command){
            manager.ExecuteIfCommand(command);            
		}

		public static void AddCommands(params CommandBase[] cs){
			foreach(CommandBase c in cs)
				AddCommand(c);
		}

		public static void AddCommand(CommandBase c){
            Singleton.manager.Add(c);
		}
		
        [System.Obsolete]
		public static void RemoveCommand(string commandName){}
		#endregion

		#region Predefined Commands
        [Command(group: "", alias:"clear", help:"Clears the console")]
		static void Clear(){
			Singleton.consoleText = new StringBuilder();
			Singleton.numLines = 0;
		}

        [Command(group: "", alias: "help", help: "Shows this message")]
        static void HelpCommand() {
            CommandBase[] commands = Singleton.manager.GetCommands();
            StringBuilder text = new StringBuilder("List of commands");
            for (int i = 0; i < commands.Length; i++) {
                text.Append(commands[i].fullSignature + (commands[i].help == null ? string.Empty : " - " + commands[i].help));
                text.Append('\n');
            }
            LogInfo("\n" + text.ToString());
        }

        [Command(alias: "showVerbose", help: "Whether or not to show extra verbose info")]
        static void ShowVerbose(bool show){
			verbose = show;
			Log("Changed Succesful", Color.green);
		}

        [Command(alias: "changeKey", help: "Changes de key used to open/close the console. Type \"changeKeyHelp\" for extra help")]
        static void ChangeKey(string key){
			//If not a number
			int n;
			if (!int.TryParse(key, out n)){
				try{
					Singleton.consoleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), key, true);
					Log("Change successful", Color.green);
				}catch{
					LogError("The entered value is not a valid KeyCode value");
				}
			}
			else{
				string[] keyCodes = System.Enum.GetNames(typeof(KeyCode));
				if (n>=0 || n<keyCodes.Length){
					try{
						Singleton.consoleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode),keyCodes[n], true);
						Log("Change successful", Color.green);
					}catch{
						LogError("The entered value is not a valid KeyCode value");
					}
				}
				else
					LogError("The entered value is not a valid KeyCode value");
			}
		}

        [Command(alias: "changeKeyHelp", help:"Lists all of the possible keys to use with the \"changeKey\" command")]
        static void ChangeKeyHelp() {
            string[] keyCodes = System.Enum.GetNames(typeof(KeyCode));
            StringBuilder help = new StringBuilder("\nSPECIAL KEYS 1: ");
            int lineLength = 0;
            StringBuilder text;
            for (int i = 0; i < keyCodes.Length; i++) {
                text = new StringBuilder();
                if (i == 22) {
                    text.Append("\n\nNUMERIC KEYS: ");
                    lineLength = 0;
                }
                else if (i == 32) {
                    text.Append("\n\nSPECIAL KEYS 2: ");
                    lineLength = 0;
                }
                else if (i == 45) {
                    text.Append("\n\nALPHA KEYS: ");
                    lineLength = 0;
                }
                else if (i == 71) {
                    text.Append("\n\nKEYPAD KEYS: ");
                    lineLength = 0;
                }
                else if (i == 89) {
                    text.Append("\n\nSPECIAL KEYS 3: ");
                    lineLength = 0;
                }
                else if (i == 98) {
                    text.Append("\n\nF KEYS: ");
                    lineLength = 0;
                }
                else if (i == 113) {
                    text.Append("\n\nSPECIAL KEYS 4: ");
                    lineLength = 0;
                }
                else if (i == 134) {
                    text.Append("\n\nMOUSE: ");
                    lineLength = 0;
                }
                else if (i == 141) {
                    text.Append("\n\nJOYSTICK KEYS: ");
                    lineLength = 0;
                }
                text.Append(string.Format("{0}[{1}]{2}", keyCodes[i], i, i != keyCodes.Length - 1 ? "," : ""));
                lineLength += text.Length;
                help.Append(text);
                if (lineLength >= 65) {
                    help.Append('\n');
                    lineLength = 0;
                }
            }
            LogInfo("Command Info: " + help.ToString());
        }

        [Command(alias: "showLog", help: "Whether or not to show Debug.Log and variants log")]
        static void ShowLog(bool value){
			if (value)
				Application.logMessageReceived+=Singleton.LogCallback;
			else
				Application.logMessageReceived-=Singleton.LogCallback; 
			Log("Change successful", Color.green);
		}

        [Command(alias: "showTimeStamp", help: "Whether or not to show a time stamp on each message")]
        static void ShowTimeStamp(bool value){
			Singleton.showTimeStamp = value;
			Log("Change successful", Color.green);
		}

        [Command(alias: "setFontSize", help: "Set the font size used in the console")]
        static void SetFontSize(int size){
            Singleton.fontSize = size;
			Log("Change successful", Color.green);
		}
		#endregion
	}

	[System.Serializable]
	struct Settings{
		public AnimationCurve curve;
		[TooltipAttribute("When checked, logs from the Debug class and exceptions will be printed to the Console")]
		public bool showDebugLog;
		[TooltipAttribute("Log additional errors information")]
		public bool logVerbose;
		[TooltipAttribute("If none is set, the default one will be used")]
		public GUISkin skin;
		[TooltipAttribute("If none is set, the default one will be used")]
		public Font font;
	}
}