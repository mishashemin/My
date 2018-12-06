using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Collections;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization;
using SharpGL.SceneGraph.Core;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;

namespace PolygonLoadingSample
{
    public partial class FormPolygonLoadingSample : Form
    {
        public FormPolygonLoadingSample()
        {
            InitializeComponent();

            //  Get the OpenGL object, for quick access.
            OpenGL gl = this.openGLControl1.OpenGL;      

            //  A bit of extra initialisation here, we have to enable textures.           
            importPolygon("C:\\Users\\Владелец\\Desktop\\Котедж\\cottage_obj.obj");
            importPolygon("C:\\Users\\Владелец\\Desktop\\Котедж\\Football.obj");
            this.MouseWheel += new MouseEventHandler(this_MouseWheel);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.databasen.se/hem/johan/raytracing/rayobj.htm");
        }

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object, for quick access.
            var gl = this.openGLControl1.OpenGL;

            //  Clear and load the identity.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();           
            //  View from a bit away the y axis and a few units above the ground.
            gl.LookAt(camera.X, camera.Y, camera.Z, direct.X, direct.Y, direct.Z, 0, 1, 0);

            //  Rotate the objects every cycle.
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1.0f, 0.0f, 0.0f); //задает цвет red
            gl.Vertex(2.0f, 0.0f, 0.0f); //задает точку             
            gl.Vertex(0.0f, 0.0f, 0.0f);//задает точку

            gl.Color(0.0f, 1.0f, 0.0f); //задает цвет greeb
            gl.Vertex(0.0f, 2.0f, 0.0f); //задает точку             
            gl.Vertex(0.0f, 0.0f, 0.0f);//задает точку

            gl.Color(0.0f, 0.0f, 1.0f); //задает цвет bloo
            gl.Vertex(0.0f, 0.0f, 2.0f); //задает точку             
            gl.Vertex(0.0f, 0.0f, 0.0f);//задает точку
            gl.End();


            //gl.Scale(0.3f, 0.3f, 0.3f);
            
            //Move the objects down a bit so that they fit in the screen better.
            //gl.Translate(40f, 0, 0);

            //  Draw every polygon in the collection.


            polygons[0].PushObjectSpace(gl);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Scale(10f, 10f, 10f);
            polygons[0].Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            polygons[0].PopObjectSpace(gl);

            //polygons[1].PopObjectSpace(gl);
            //gl.Translate(0f, 0, 0);
            //polygons[1].Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            //polygons[1].PopObjectSpace(gl);


            //gl.Scale(0.1f, 0.1f, 0.1f);
            //  Rotate a bit more each cycle.
            
                
        }
        

        //  A set of polygons to draw.
        List<Polygon> polygons = new List<Polygon>();

        //  The camera.
        Texture texture = new Texture();

        Vertex camera = new Vertex(-25, 25, -25);
        Vertex direct = new Vertex(0, 0, 0);
        /// <summary>
        /// Handles the Click event of the importPolygonToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Resize(Polygon poligon, float scale)
        {
            poligon.Transformation.ScaleX *= scale;
            poligon.Transformation.ScaleY *= scale;
            poligon.Transformation.ScaleZ *= scale;
        }

        private void importPolygon(string filename)
        {
            Scene scene = SerializationEngine.Instance.LoadScene(filename);
            if (scene != null)
            {
                foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                {
                    //  Get the bounds of the polygon.
                    BoundingVolume boundingVolume = polygon.BoundingVolume;
                    float[] extent = new float[3];
                    polygon.BoundingVolume.GetBoundDimensions(out extent[0], out extent[1], out extent[2]);

                    //  Get the max extent.
                    float maxExtent = extent.Max();

                    //  Scale so that we are at most 10 units in size.
                    float scaleFactor = maxExtent > 10 ? 10.0f / maxExtent : 1;
                    polygon.Transformation.ScaleX = scaleFactor;
                    polygon.Transformation.ScaleY = scaleFactor;
                    polygon.Transformation.ScaleZ = scaleFactor;
                    polygon.Freeze(openGLControl1.OpenGL);
                    polygons.Add(polygon);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the freezeAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void freezeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var poly in polygons)
                poly.Freeze(openGLControl1.OpenGL);
        }

        /// <summary>
        /// Handles the Click event of the unfreezeAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void unfreezeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var poly in polygons)
                poly.Unfreeze(openGLControl1.OpenGL);
        }

        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = this.openGLControl1.OpenGL;
            wireframeToolStripMenuItem.Checked = false;
            solidToolStripMenuItem.Checked = false;
            lightedToolStripMenuItem.Checked = true;
            openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            
        }
        
        void WireframeToolStripMenuItemClick(object sender, EventArgs e)
        {
        	wireframeToolStripMenuItem.Checked = true;
        	solidToolStripMenuItem.Checked = false;
			lightedToolStripMenuItem.Checked = false;
        	openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
        	openGLControl1.OpenGL.Disable(OpenGL.GL_LIGHTING);
        }
        
        void SolidToolStripMenuItemClick(object sender, EventArgs e)
        {
        	wireframeToolStripMenuItem.Checked = false;
        	solidToolStripMenuItem.Checked = true;
        	lightedToolStripMenuItem.Checked = false;
        	openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);        	
        	openGLControl1.OpenGL.Disable(OpenGL.GL_LIGHTING);
        }
        
        void LightedToolStripMenuItemClick(object sender, EventArgs e)
        {
            var gl = this.openGLControl1.OpenGL;
            wireframeToolStripMenuItem.Checked = false;
        	solidToolStripMenuItem.Checked = false;
        	lightedToolStripMenuItem.Checked = true;
            openGLControl1.OpenGL.PolygonMode(FaceMode.Front, PolygonMode.Filled);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, Light3);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, worldLightPosition);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_CONSTANT_ATTENUATION, 1);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_LINEAR_ATTENUATION, 0.09f);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_QUADRATIC_ATTENUATION, 0.032f);
            openGLControl1.OpenGL.Enable(OpenGL.GL_LIGHTING);
            openGLControl1.OpenGL.Enable(OpenGL.GL_LIGHT2);
            //openGLControl1.OpenGL.Enable(OpenGL.GL_COLOR_MATERIAL);

        }

        private Vertex worldLightPosition = new Vertex(0f, 5f, -100f);
        private Vertex Light3 = new Vertex(1f, 1f, 1f);
        void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
        	Close();
        }
        
        void ClearToolStripMenuItemClick(object sender, EventArgs e)
        {
        	polygons.Clear();
        }

     

        private void this_MouseWheel(object sender, MouseEventArgs e)
        {
            var velocity = direct - camera;
            velocity.Normalize();
            
            if (e.Delta>0)
            {
                direct += velocity;
                camera += velocity;
            }
            else
            {
                direct -= velocity;
                camera -= velocity;
            }
            
        }
    }
}