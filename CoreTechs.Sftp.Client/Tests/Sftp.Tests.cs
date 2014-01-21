﻿using NUnit.Framework;
using System;
using System.Data.Common;
using System.IO;

namespace CoreTechs.Sftp.Client.Tests
{
    class SFTPTests
    {
        private const string Cs =
            "username=pcsi;password=pcsi;host=localhost;hostkey=ssh-dss 1024 88:89:38:e8:d0:c4:f4:46:a4:b0:2b:7d:e0:ba:e9:fb";

        [Test]
        //[Repeat(10)]
        public void CanMakeAndDeleteADummyFile()
        {
            const int mb = 1048576;
            const int twentyMb = mb * 20;
            FileInfo theFile;
            using (var dummy = new DummyFile(twentyMb))
            {
                theFile = dummy.FileInfo;
                Assert.That(theFile.Exists, Is.True);
                Assert.That(theFile.Length, Is.EqualTo(twentyMb));
            }
            theFile.Refresh();
            Assert.That(theFile.Exists, Is.False);
        }

        [Test]
        //[Repeat(10)]
        public void CanUploadToRootDir()
        {
            using (var file = new DummyFile(1024 * 1024 * 3))
            {
                Sftp.UploadFile(Cs, file.FileInfo.Name);
            }
        }
        [Test]
        //[Repeat(10)]
        public void CanUpload2FilesToRootDir()
        {
            using (var file1 = new DummyFile(1024 * 1024 * 3))
            using (var file2 = new DummyFile(1024 * 1024 * 3))
            {
                Sftp.UploadFiles(Cs, file1.FileInfo.DirectoryName, "*.*");
            }
        }
        [Test]
        //[Repeat(10)]
        public void CanUploadFileToRootDirUsingProperties()
        {
            using (var file = new DummyFile(1024 * 1024 * 3))
            {
                var csb = new DbConnectionStringBuilder { ConnectionString = Cs };
                Sftp.Host = csb["host"] as string;
                Sftp.Username = csb["username"] as string;
                Sftp.Password = csb["password"] as string;
                Sftp.Hostkey = csb["hostkey"] as string;
                Sftp.Port = csb["port"] as string;

                Sftp.UploadFile(file.FileInfo.Name);
            }
        }
        [Test]
        //[Repeat(10)]
        public void CanUpload2FilesToRootDirUsingProperties()
        {
            using (var file1 = new DummyFile(1024 * 1024 * 3))
            using (var file2 = new DummyFile(1024 * 1024 * 3))
            {
                var csb = new DbConnectionStringBuilder { ConnectionString = Cs };
                Sftp.Host = csb["host"] as string;
                Sftp.Username = csb["username"] as string;
                Sftp.Password = csb["password"] as string;
                Sftp.Hostkey = csb["hostkey"] as string;
                Sftp.Port = csb["port"] as string;

                Sftp.UploadFiles(file1.FileInfo.DirectoryName, "*.*");
            }
        }

        [Test]
        //[Repeat(10)]
        public void CanUploadToSubDir()
        {
            using (var file = new DummyFile(1024 * 1024 * 3))
            {
                Sftp.UploadFile(Cs, file.FileInfo.Name, remoteDirectoryPath: "subdir");
            }
        }

        [Test]
        //[Repeat(10)]
        public void CanUploadToSubSubDir()
        {
            using (var file = new DummyFile(1024 * 1024 * 3))
            {
                Sftp.UploadFile(Cs, file.FileInfo.Name, remoteDirectoryPath: "a/b/c");
            } 
        }
    }

    internal class DummyFile : IDisposable
    {
        public FileInfo FileInfo { get; set; }

        public DummyFile(int byteLength)
        {
            FileInfo = new FileInfo(Path.GetTempFileName());
            File.WriteAllBytes(FileInfo.FullName, new byte[byteLength]);
        }

        public void Dispose()
        {
            try
            {
                FileInfo.Delete();
            }
            catch { }
        }
    }
}
