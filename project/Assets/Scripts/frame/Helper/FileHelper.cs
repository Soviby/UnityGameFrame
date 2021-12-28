using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


 
    public static class FileHelper
    {
        public static void CleanDirectory(string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                Directory.Delete(subdir, true);
            }

            foreach (string subFile in Directory.GetFiles(dir))
            {
                File.Delete(subFile);
            }
        }

        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }

        public static void GetAllFiles(List<string> files, string dir)
        {
            string[] fls = Directory.GetFiles(dir);
            foreach (string fl in fls)
            {
                files.Add(fl);
            }

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                GetAllFiles(files, subDir);
            }
        }

        public delegate bool WalkTreeAction(string filePath);

        public static bool _WalkTree(string root, string dir, WalkTreeAction action)
        {
            foreach (string subDir in Directory.GetDirectories(dir))
            {
                if (!_WalkTree(root, subDir, action))
                {
                    return false;
                }
            }

            foreach (string filePath in Directory.GetFiles(dir))
            {
                var path = filePath.Substring(root.Length);
                path = path.Replace("\\", "/");
                if (path.StartsWith("/")) path = path.Substring(1);
                if (!action(path))
                {
                    return false;
                }
            }

            return true;
        }

        public static void WalkTree(string root, WalkTreeAction action)
        {
            _WalkTree(root, root, action);
        }

        public static string BlobbedPath(string filename)
        {
            if (filename.Length <= 2)
            {
                return "__/" + filename;
            }

            return filename.Substring(0, 2) + "/" + filename;
        }

        public static bool CheckFileSize(string path, long size)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            FileInfo Files = new FileInfo(path);
            long fsize = Files.Length;
            if (fsize != size)
            {
                return false;
            }

            return true;
        }

        public static bool CheckFileHashAndSize(string path, long size, string hash)
        {
            if (!CheckFileSize(path, size))
            {
                return false;
            }

            string fhash = GetFileHash(path);
            if (fhash != hash)
            {
                return false;
            }

            return true;
        }


        public static void SyncFile(string srcFile, string dstFile)
        {
            bool match = false;
            if (File.Exists(dstFile))
            {
                var srcSize = GetFileSize(srcFile);
                var dstSize = GetFileSize(dstFile);
                if (srcSize == dstSize)
                {
                    var srcHash = GetFileHash(srcFile);
                    var dstHash = GetFileHash(dstFile);
                    if (srcHash == dstHash)
                    {
                        match = true;
                    }
                }
            }
            if (!match)
            {
                CopyFile(srcFile, dstFile, true);
            }
        }

        public static bool CopyFile(string src, string dest, bool overwrite = false)
        {
            if (!File.Exists(src))
            {
                return false;
            }
            string dir = Path.GetDirectoryName(dest);
            PrepareDirectory(dir);
            File.Copy(src, dest, overwrite);
            return true;
        }

        public static string GetFileHash(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (MD5 md5Hash = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] data = md5Hash.ComputeHash(stream);
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    return sBuilder.ToString();
                }
            }
        }

        public static bool GetFileHashAndSize(string path, out long size, out string hash)
        {
            if (!File.Exists(path))
            {
                size = 0;
                hash = null;
                return false;
            }

            FileInfo Files = new FileInfo(path);
            size = Files.Length;
            hash = GetFileHash(path);
            return true;
        }

        public static long GetFileSize(string path)
        {
            if (!File.Exists(path))
            {
                return 0;
            }

            FileInfo info = new FileInfo(path);
            return info.Length;
        }

        public static void PrepareDirectory(string dir, bool clearDir = false)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (clearDir)
            {
                FileHelper.CleanDirectory(dir);
            }
        }


        public static void ForeachAssets(string root, FileHelper.WalkTreeAction action)
        {
            FileHelper.WalkTree(root, path =>
            {
                if (!path.EndsWith(".meta"))
                {
                    var metaFile = Path.Combine(root, path + ".meta");
                    if (File.Exists(metaFile))
                    {
                        action(path);
                    }
                }
                return true;
            });
        }


    }

