using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UIDock;

public class Test : MonoBehaviour
{
    public UIDocument uiDocument;

    private Root _root;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UI Document is not assigned.");
            return;
        }

        // Initialize Styles
        var styleSheet = Resources.Load<StyleSheet>("DockStyles");
        
        _root = new Root();
        if (styleSheet != null) _root.styleSheets.Add(styleSheet);
        
        uiDocument.rootVisualElement.Add(_root);

        // Add some test windows
        _root.AddWindow("Inspector");
        _root.AddWindow("Hierarchy");
        _root.AddWindow("Project");
        _root.AddWindow("Console");

        // Split them up for demo (Hardcoded layout for now)
        if (_root.RootDock != null && _root.RootDock.dockType == Dock.Type.Window)
        {
            var win1 = _root.RootDock;
            var win2 = new Dock(new DockWindow("Scene"));
            
            // Create a Horizontal Split
            var container = new Dock(Dock.Type.Horizontal, win1, win2);
            _root.RootDock = container;
            _root.RequestLayoutUpdate();
        }
    }
    
    // Simple UI to add windows
    void OnGUI()
    {
        if (GUILayout.Button("Add Window"))
        {
            _root.AddWindow("New Window " + Time.frameCount);
        }
    }
}
