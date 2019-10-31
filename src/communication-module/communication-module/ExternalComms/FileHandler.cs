using System;
using System.Collections.Generic;
using System.IO;
using Core.ImageReceiver;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.connection;
using File = custom_message_based_implementation.model.File;

namespace Core.ExternalComms
{
    internal class FileHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private FileServermoduleProxy FileServermoduleProxy { get; }

        private List<FileName> _listOfFileNames;

        internal FileHandler(ProxyHelper ph, ClientModuleCommunication cmm)
        {
            this.FileServermoduleProxy = new FileServermoduleProxy(ph, cmm);
        }

        internal string GetListOfFiles(PrimaryKeyWrapper parameters)
        {
            _listOfFileNames = null;
            Logger.Info("GetListOfFiles initiated, " + nameof(_listOfFileNames) + " set to null");
            FileServermoduleProxy.GetListOfFiles(new PrimaryKey{TheKey = parameters.PrimaryKey}, GetListOfFilesCallback);
            GeneralHandler.PollVariableFor10Seconds(ref _listOfFileNames);
            return GeneralHandler.ReturnAsJSON(_listOfFileNames);
        }

        internal string UploadFile(PrimaryKeyAndFileWrapper parameters)
        {
            Logger.Info("UploadFile initiated");
            var fileData = System.IO.File.ReadAllBytes(parameters.FileName); // file name is an absolute path of the uploaded file
            var actualFileName = Path.GetFileName(parameters.FileName);
            FileServermoduleProxy.UploadFile(new File{FileData = fileData, FileName = new FileName{FileNameProp = actualFileName}}, new PrimaryKey { TheKey = parameters.PrimaryKey }, false, UploadFileCallback);
            return "Done (UploadFile)";
        }

        internal string DownloadFile(PrimaryKeyAndFileWrapper parameters)
        {
            Logger.Info("DownloadFile initiated");
            FileServermoduleProxy.DownloadFile(new FileName{FileNameProp = parameters.FileName}, new PrimaryKey{TheKey = parameters.PrimaryKey}, DownloadFileCallback);
            return "Done (DownloadFile)";
        }

        internal string RenameFile(RenameFileWrapper parameters)
        {
            Logger.Info("RenameFile initiated");
            FileServermoduleProxy.RenameFile(new FileName { FileNameProp = parameters.OldFileName }, new FileName { FileNameProp = parameters.NewFileName }, new PrimaryKey { TheKey = parameters.PrimaryKey }, RenameFileCallback);
            return "Done (RenameFile)";
        }

        // Callbacks
        private void GetListOfFilesCallback(List<FileName> fileNames)
        {
            _listOfFileNames = fileNames;
            Logger.Info(this._listOfFileNames.GetType().Name + " " + nameof(this._listOfFileNames) + " set (GetListOfFilesCallback)"); 
        }

        private void UploadFileCallback()
        {
            Logger.Info("Upload of file executed (UploadFileCallback)");
        }

        private void DownloadFileCallback(File file)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", file.FileName.FileNameProp);
            Logger.Debug("Overwriting " + path);
            System.IO.File.WriteAllBytes(path, file.FileData);
            Logger.Info("Download of file executed(DownloadFileCallback)");
        }

        private void RenameFileCallback()
        {
            Logger.Info("Rename of file executed (RenameFileCallback)");
        }
    }
}