using System;
using Android.App;
using Android.Hardware;
using Android.Os;
using Android.Util;
using Android.View;
using Android.Widget;
using Dot42.Manifest;
using Java.Io;
using Java.Text;
using Java.Util;
using Environment = Android.Os.Environment;

[assembly: Application("dot42 Simple Menu", Icon = "Icon")]
[assembly: UsesFeature("android.hardware.camera")]
[assembly: UsesPermission(Android.Manifest.Permission.CAMERA)]
[assembly: UsesPermission(Android.Manifest.Permission.WRITE_EXTERNAL_STORAGE)]

namespace SimpleMenu
{
    [Activity(ScreenOrientation = ScreenOrientations.Landscape)]
    public class MainActivity : Activity
    {
        private TextView tbLog;

        protected override void OnCreate(Bundle savedInstance)
        {
            base.OnCreate(savedInstance);
            SetContentView(R.Layouts.MainLayout);
            tbLog = FindViewById<TextView>(R.Ids.tbLog);
            var camera = GetCamera();
            var preview = FindViewById<FrameLayout>(R.Ids.preview);
            var captureButton = FindViewById<Button>(R.Ids.captureButton);

            if (camera != null)
            {
                var cameraPreview = new CameraPreview(this, camera);
                Camera.Parameters parameters = camera.GetParameters();
                preview.AddView(cameraPreview);
                camera.StartFaceDetection();

                captureButton.Click += (s, x) => {
                    camera.TakePicture(null, null, new PictureCallback());
                };
            }
            else
            {
                preview.AddView(new TextView(this) { Text = "No camera found" });
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(R.Menus.Menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            var camera = GetCamera();

            switch (item.GetItemId())
            {
                case R.Ids.item1:
                    tbLog.Text = "item1 has been clicked";
                    break;
                case R.Ids.groupItem1:
                    tbLog.Text = "groupItem1 has been clicked";
                    break;
                case R.Ids.submenu_item1:
                    tbLog.Text = "submenu_item1 has been clicked";
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        private static Camera GetCamera()
        {
            try
            {
                return Camera.Open();
            }
            catch
            {
                return null;
            }
        }

        private class PictureCallback : Camera.IPictureCallback
        {
            public void OnPictureTaken(sbyte[] data, Camera camera)
            {
                var pictureFile = GetOutputMediaFile();
                if (pictureFile == null)
                {
                    return;
                }
                try
                {
                    var fos = new FileOutputStream(pictureFile);
                    fos.Write(data);
                    fos.Close();
                }

                catch
                {
                    // Ignore for now
                }
            }
        }

        private static File GetOutputMediaFile()
        {
            var mediaStorageDir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES), "dot42camera");
            if (!mediaStorageDir.Exists())
            {
                if (!mediaStorageDir.Mkdirs())
                {
                    Log.E("camera", "failed to create directory");
                    return null;
                }
            }
            // Create a media file name
            var timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            var mediaFile = new File(mediaStorageDir.Path + File.Separator + "kakka_" + timeStamp + ".jpg");

            return mediaFile;
        }
    }
}
