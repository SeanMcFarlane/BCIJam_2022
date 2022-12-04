using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.Assertions;

namespace Assets.LSL4Unity.EditorExtensions {
	public class LSLEditorIntegration {
		public static readonly string wikiURL = "https://github.com/xfleckx/LSL4Unity/wiki";
		public static readonly string lib64Name = "liblsl64";
		public static readonly string lib32Name = "liblsl32";

		public const string DLL_ENDING = ".dll";
		public const string SO_ENDING = ".so";
		public const string BUNDLE_ENDING = ".bundle";

		static readonly string wrapperFileName = "LSL.cs";
		static readonly string assetSubFolder = "LSL4Unity";
		static readonly string libFolder = "Plugins";

		[MenuItem("LSL/Show Streams")]
		static void OpenLSLWindow() {
			var window = EditorWindow.GetWindow<LSLShowStreamsWindow>(true);

			window.Init();

			window.ShowUtility();
		}

		[MenuItem("LSL/Show Streams", true)]
		static bool ValidateOpenLSLWindow() {
			string assetDirectory = Application.dataPath;

			bool lib64Available = false;
			bool lib32Available = false;
			bool apiAvailable = false;


			var results = Directory.GetDirectories(assetDirectory, assetSubFolder, SearchOption.AllDirectories);

			Assert.IsTrue(results.Any(), "Expecting a directory named: '" + assetSubFolder + "' containing the content inlcuding this script! Did you renamed it?");

			var root = results.Single();

			lib32Available = File.Exists(Path.Combine(root, Path.Combine(libFolder, lib32Name + DLL_ENDING)).Replace("\\", "/"));
			lib64Available = File.Exists(Path.Combine(root, Path.Combine(libFolder, lib64Name + DLL_ENDING)).Replace("\\", "/"));

			//.SO files do not exist in this repo it seems 
			//lib32Available &= File.Exists(Path.Combine(root, Path.Combine(libFolder, lib32Name + SO_ENDING)).Replace("\\", "/"));
			//lib64Available &= File.Exists(Path.Combine(root, Path.Combine(libFolder, lib64Name + SO_ENDING)).Replace("\\", "/"));

			//lib32Available &= File.Exists(Path.Combine(root, Path.Combine(libFolder, lib32Name + BUNDLE_ENDING)).Replace("\\", "/"));
			//lib64Available &= File.Exists(Path.Combine(root, Path.Combine(libFolder, lib64Name + BUNDLE_ENDING)).Replace("\\", "/"));

			apiAvailable = File.Exists(Path.Combine(root, wrapperFileName).Replace("\\", "/"));

			if((lib64Available || lib32Available) && apiAvailable)
				return true;

			Debug.LogError("LabStreamingLayer libraries not available! See " + wikiURL + " for installation instructions");
			return false;
		}

	}
}