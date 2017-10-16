using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

// Default GUI Styles 
public enum DefaultGUIStyles {
    normal = 0,
    mimicAxis = 1,
    custom = 2,
}

public enum DefaultTextSizes {
    h1 = 32,
    h2 = 24,
    h3 = 18,
    h4 = 16,
    h5 = 14,
    normal = 11
}

public class CustomizationWindow : EditorWindow {

    private SettingsSerializer settings = new SettingsSerializer();
    private DefaultGUIStyles styles;
    private int toolbar = 0;

    [MenuItem("Window/Custom Tools")]
    public static void OpenWindow() {
        EditorWindow.GetWindow<CustomizationWindow>("Custom Tools", true);
    }

    private void OnEnable() {
        settings = SettingsSerializer.Load();
    }

    private void OnGUI() {

        toolbar = GUILayout.Toolbar(toolbar, new string[] { "Move Tool", "Rotate Tool", "Scale Tool" });

        switch (toolbar) {

            #region Move Tool
            case 0:
                GUILayout.Space(20);
                GUILayout.Label("Move Tool Ruler Color", GetGUIStyle(DefaultTextSizes.h4, Color.black, FontStyle.Bold));
                GUILayout.Space(2);
                GUILayout.Label("Select your prefered colors for the ruler", GetGUIStyle(DefaultTextSizes.normal, Color.black));
                GUILayout.Space(10);

                GUILayout.BeginHorizontal(
                    GUILayout.MinWidth(400),
                    GUILayout.MaxWidth(600)
                );

                //GUILayout.Label("Preview Image Here", GetGUIStyle(DefaultTextSizes.normal, Color.black, FontStyle.Bold));
                Texture2D rulerImage;
                rulerImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/BetterToolsEditor/Resources/Preview/MoveToolPreview.jpg", typeof(Texture2D));
                GUILayout.Label(rulerImage, GUILayout.Width(200), GUILayout.Height(200));

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("X Axis Color:");
                settings.moveTool.x = EditorGUILayout.ColorField(settings.moveTool.x);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Axis Color:");
                settings.moveTool.y = EditorGUILayout.ColorField(settings.moveTool.y);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Axis Color:");
                settings.moveTool.z = EditorGUILayout.ColorField(settings.moveTool.z);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                settings.moveTool.ruler = GUILayout.Toggle(settings.moveTool.ruler, " Enable Ruler");
                settings.moveTool.label = GUILayout.Toggle(settings.moveTool.label, " Enable Label");

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                break;

            #endregion

            #region Rotate Tool
            case 1:
                GUILayout.Space(20);
                GUILayout.Label("Rotate Tool Disc Color", GetGUIStyle(DefaultTextSizes.h4, Color.black, FontStyle.Bold));
                GUILayout.Space(2);
                GUILayout.Label("Select your prefered colors for the axis rotation", GetGUIStyle(DefaultTextSizes.normal, Color.black));
                GUILayout.Space(10);

                GUILayout.BeginHorizontal(
                    GUILayout.MinWidth(400),
                    GUILayout.MaxWidth(600)
                );

                Texture2D rotateImage;
                rotateImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/BetterToolsEditor/Resources/Preview/RotateToolPreview.jpg", typeof(Texture2D));
                GUILayout.Label(rotateImage, GUILayout.Width(200), GUILayout.Height(200));

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("X Axis Color:");
                settings.rotateTool.x = EditorGUILayout.ColorField(settings.rotateTool.x);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Axis Color:");
                settings.rotateTool.y = EditorGUILayout.ColorField(settings.rotateTool.y);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Axis Color:");
                settings.rotateTool.z = EditorGUILayout.ColorField(settings.rotateTool.z);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                settings.rotateTool.ruler = GUILayout.Toggle(settings.rotateTool.ruler, " Enable Ruler");
                settings.rotateTool.label = GUILayout.Toggle(settings.rotateTool.label, " Enable Label");

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                break;
            #endregion

            #region Scale Tool
            case 2:

                GUILayout.Space(20);
                GUILayout.Label("Rotate Tool Disc Color", GetGUIStyle(DefaultTextSizes.h4, Color.black, FontStyle.Bold));
                GUILayout.Space(2);
                GUILayout.Label("Select your prefered colors for the axis rotation", GetGUIStyle(DefaultTextSizes.normal, Color.black));
                GUILayout.Space(10);

                GUILayout.BeginHorizontal(
                    GUILayout.MinWidth(400),
                    GUILayout.MaxWidth(600)
                );

                Texture2D scaleImage;
                scaleImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/BetterToolsEditor/Resources/Preview/ScaleToolPreview.jpg", typeof(Texture2D));
                GUILayout.Label(scaleImage, GUILayout.Width(200), GUILayout.Height(200));

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("X Axis Color:");
                settings.scaleTool.x = EditorGUILayout.ColorField(settings.scaleTool.x);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Axis Color:");
                settings.scaleTool.y = EditorGUILayout.ColorField(settings.scaleTool.y);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Axis Color:");
                settings.scaleTool.z = EditorGUILayout.ColorField(settings.scaleTool.z);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                settings.scaleTool.ruler = GUILayout.Toggle(settings.scaleTool.ruler, " Enable Ruler");
                settings.scaleTool.label = GUILayout.Toggle(settings.scaleTool.label, " Enable Label");

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                break;
            #endregion
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal(
            GUILayout.MinWidth(400),
            GUILayout.MaxWidth(600)
        );

        if (GUILayout.Button("Save Changes", GUILayout.Width(100), GUILayout.Height(20))) {
            settings.Save();
        }

        GUILayout.EndHorizontal();
    }

    private GUIStyle GetGUIStyle(DefaultTextSizes size, Color color, FontStyle textStyle = FontStyle.Normal) {
        RectOffset _offset = GUI.skin.button.padding;

        GUIStyle style = new GUIStyle();
        style.fontSize = (int)size;
        style.normal.textColor = color;
        style.fontStyle = textStyle;
        style.margin = _offset;

        return style;
    }
}
