﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace LotusEngine
{
    public static class Core
    {
        private static bool initialized = false;
        internal static Action newFrameBegins;

        /// <summary>
        /// Whether the engine has been initialized yet.
        /// </summary>
        public static bool Initialized { get { return initialized; } }
        public static void InitializeEngine(OpenGL gl, int width, int height)
        {
            if (initialized)
            {
                Debug.LogError("Engine has already been initialized!");
                return;
            }

            // Subscribe methods to frame start
            newFrameBegins += Time.NewFrameBegins;
            newFrameBegins += Input.NewFrameBegins;
            newFrameBegins += Scene.NewFrameBegins;

            // Settings
            Settings.Screen.Width = width;
            Settings.Screen.Height = height;

            // Load and bind textures
            Rendering.InitializeGraphics(gl);

            // TODO Load sounds
            Sprite.LoadAllSprites();
            Prefab.LoadAllPrefabs();
            Scene.LoadAllScenes();

            initialized = true;
        }

        public static void Update(bool updateObjects = true)
        {
            if (!initialized)
            {
                Debug.LogError("Cannot run Core.Update(); engine has not been initialized yet.");
                return;
            }
            
            newFrameBegins();

            if (updateObjects)
                foreach (var go in Scene.ActiveScene.sceneObjects)
                    go.Update();

            if (updateObjects)
                Collider.CollisionCheckAll();
        }

        public static void Draw(View useView = null, bool drawGui = true)
        {
            if (!initialized)
            {
                Debug.LogError("Cannot run Core.Draw(); engine has not been initialized yet.");
                return;
            }

            OpenGL gl = Rendering.gl;

            //  Set the clear color.
            var color = Scene.ActiveScene.bgColor.GetRGBAFloats();
            gl.ClearColor(color[0], color[1], color[2], color[3]);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(MatrixMode.Projection);

            if (useView != null)
            {
                DrawView(useView);
            }
            else
            {
                foreach (var view in Scene.ActiveScene.views)
                {
                    DrawView(view);
                }
            }
        }

        private static void DrawView(View view)
        {
            OpenGL gl = Rendering.gl;

            Scene.ActiveScene.SetCurrentView(view);

            //Rendering.gl.LoadIdentity();
            //Rendering.gl.Translate(view.screenX, view.screenY, 0);
            //Rendering.gl.Scale(view.width / Settings.Screen.Width, view.height / Settings.Screen.Height, 1);
            //Rendering.FillRectangle(0, 0, Settings.Screen.Width, Settings.Screen.Height, Scene.ActiveScene.bgColor);

            foreach (var go in Scene.ActiveScene.sceneObjects)
            {
                go.Draw();
            }
        }
    }
}
