Currently, Improved Tools adds support for Advanced Handles. In this file you will find an example of how to implement
a custom handle.

In the default Tool script (Move, Rotate, Scale, or your own Custom script), include the advanced handles namespace.

// using AdvancedHandles;

Once done you can call the handle in the OnSceneGUI fucntion with the following lines of code.
Also remember to hide any handles that you are already drawing.

// AHandles.Toolname(
// 	startPosition,
//	_handle.transform.position,
//	Quaternion.LookRotation(Camera.current.transform.position - startPosition).eulerAngles.y * -1
//	);

Done. The handle should now show up when using a tool.