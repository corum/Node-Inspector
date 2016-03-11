﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector.Editor{	
    public enum ConnectionRenderType{
        OutputToInput,
        MouseToInputNode,
        OutputNodeToMouse
    }

    public class Connection{
        const float BezierMinDistanceForSelection = 3.0f;

        public int      ControlID   {get; private set;}
        public Joint    InputJoint  {get; set;}
		public Joint    OutputJoint {get; set;}
        public bool     Focused     {get; private set;}

        public ConnectionRenderType ConnectionType {get; set;}


        public static Connection GetInstance(int ControlID){            
            Connection result = (Connection)GUIUtility.GetStateObject(typeof(Connection), ControlID);
            result.ControlID = ControlID;
            return result;
        }

        public void OnGUI(){

            if (Event.current.type == EventType.mouseDown){  
                Focused = false;             
                if (HandleUtility.DistancePointBezier(Event.current.mousePosition, OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                    OutputJoint.BezierNormal, InputJoint.BezierNormal) <= BezierMinDistanceForSelection){                   
                    GUIUtility.hotControl = ControlID;
                    Focused = true;
                    Event.current.Use();
                }    
            }


            switch(Event.current.GetTypeForControl(ControlID)){
                case EventType.Repaint:
                    {                 
                        Focused = Focused && ControlID == GUIUtility.hotControl;
                        float width = 2.0f;
                        Color color = Focused ? Color.black : Color.gray;

                        Handles.BeginGUI();

                        switch (ConnectionType){
                            case ConnectionRenderType.OutputToInput:
                                {
                                    Handles.DrawBezier (OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                                        OutputJoint.BezierNormal, InputJoint.BezierNormal, color, null, width);                                                
                                    break;
                                }
                            case ConnectionRenderType.MouseToInputNode:
                                {
                                    Handles.DrawBezier (Event.current.mousePosition, InputJoint.BezierSidePoint,
                                        OutputJoint.BezierNormal, InputJoint.BezierNormal, color, null, width);                                                
                                    break;
                                }
                            case ConnectionRenderType.OutputNodeToMouse:
                                {
                                    Handles.DrawBezier (OutputJoint.BezierSidePoint, Event.current.mousePosition,
                                        OutputJoint.BezierNormal, InputJoint.BezierNormal, color, null, width);                                                
                                    break;
                                }
                        }


                        Handles.EndGUI();
                        break;
                    }                   
                case EventType.KeyDown:
                    {
                        if (GUIUtility.hotControl == ControlID 
                            && (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)){
                            InputJoint.ClearJointLink();
                            OutputJoint.ClearJointLink();
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;
                    }
            }
        }
	}
}