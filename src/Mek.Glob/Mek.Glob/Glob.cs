using System;
using System.Collections.Generic;
using System.IO;
//using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mek.Glob
{
	public class Glob
	{
		public static readonly char DirectorySeparator = '/';
		public static readonly Regex IsVolumeDriveRootRE = new Regex(@"^[a-zA-Z]\" + Path.VolumeSeparatorChar + "$", RegexOptions.Singleline);
		public static readonly Regex HasVolumeDriveRootRE = new Regex(@"^[a-zA-Z]\" + Path.VolumeSeparatorChar, RegexOptions.Singleline);

		public static IEnumerable<string> Expand(string pattern)
		{
			// nothing ever matches for the empty
			if (string.IsNullOrWhiteSpace(pattern))
				yield break;

			var tokens = pattern.Split(DirectorySeparator);

			// ensure we dont have any empt strings in the path
			if (tokens.Any(string.IsNullOrWhiteSpace))
				throw new ArgumentException("invalid pattern, pattern contains a segment that is empty");

			var queue = new Queue<string>(tokens);
			
			// if its only 1 token in the path, just return
			// the files that match no need to walk the tree
			if (queue.Count == 1)
			{
				var token = queue.Dequeue();
				var files = Expand(string.Empty, token);

				foreach (var file in files)
					yield return file;

				yield break;
			}

			var paths = new Stack<string>(10);

			// this will be use to save a list of new paths to walk
			var newPaths = new List<string>(10);

			// pull the tokens one at the time and 
			// walk the matching directories until we
			// have no more tokens, at this point we return 
			// anything that matches
			while (queue.Count > 0)
			{
				var token = queue.Dequeue();

				// if we have no paths, just expand any matches
				// for this first token and put them into the paths to 
				// walk
				if (paths.Count == 0)
				{
					var files = Expand(string.Empty, token);

					foreach (var file in files)
						paths.Push(AddVolumeDriveSeparator(file));

					continue;
				}

				// pop the last pathes from the stack and expand any matches
				// matches are then stored i the newPaths var. 
				// after the paths stack is consume, we add the newPaths
				// to walk and match for then next token
				while (paths.Count > 0)
				{
					var path = paths.Pop();
					var files = Expand(path, token);

					foreach (var file in files)
					{
						// we reach the end, no more tokens, return file matches
						if (queue.Count == 0)
							yield return file;
						else
							newPaths.Add(file);
					}
				}

				// added then new paths to the stack of paths to visit
				foreach (var newPath in newPaths)
					paths.Push(newPath);

				newPaths.Clear();
			}
		}

		private static IEnumerable<string> Expand(string dirPath, string pattern)
		{
			// handle the case where we are expanding a root drive
			if (string.IsNullOrWhiteSpace(dirPath) && IsVolumeDriveRoot(pattern) && Directory.Exists(pattern))
			{
				yield return pattern;
				yield break;
			}

			// dirpath defaults to current dir if not set / provided in glob
			if (string.IsNullOrWhiteSpace(dirPath))
				dirPath = Environment.CurrentDirectory;

			var files = Enumerable.Empty<string>();

			// handle recursive dir glob
			if (pattern == "**")
			{
				files = Directory.EnumerateDirectories(dirPath, "*", SearchOption.AllDirectories);
			}
			else if (pattern == "..")
			{
				var parent = Path.GetDirectoryName(dirPath);
				if (parent != null)
					files = new[] { parent };
			}
			else
			{
				files = Directory.EnumerateFileSystemEntries(dirPath);
			}

			// iterate over results and return any where the filename match the glob pattern.
			var glob = new GlobPattern(pattern);

			foreach (var file in files)
			{
				var filename = Path.GetFileName(file);

				if (pattern == "..")
					yield return file;
				else if (glob.IsMatch(filename))
					yield return file;
			}
		}

		private static string AddVolumeDriveSeparator(string path)
		{
			var hasRoot = HasVolumeDriveRoot(path);
			var nextCharIsNotDirSeparator = !(path.Length >= 3 && path[2] == DirectorySeparator);

			if (hasRoot && nextCharIsNotDirSeparator)
				return path.Insert(2, DirectorySeparator.ToString());
			else
				return path;
		}

		private static bool IsVolumeDriveRoot(string path)
		{
			return IsVolumeDriveRootRE.IsMatch(path); 
		}

		private static bool HasVolumeDriveRoot(string path)
		{
			return HasVolumeDriveRootRE.IsMatch(path);
		}
	}
}