using System;
using DotVVM.Framework.Runtime.Filters;
using System.Linq;
using System.Threading.Tasks;
using CheckBook.App.Helpers;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Storage;

namespace CheckBook.App.ViewModels
{
    [Authorize]
	public class SettingsViewModel : AppViewModelBase
    {
        private readonly IUploadedFileStorage uploadedFileStorage;
        private readonly FileStorageHelper fileStorageHelper;
        private readonly UserService userService;


        public UserInfoData Data { get; set; }

        public string AlertText { get; set; }

        public string AlertType { get; set; }


        // To make file uploads work you have to register the IUploadedFilesStorage in the Startup.cs file
        public UploadedFilesCollection AvatarFiles { get; set; } = new UploadedFilesCollection();

        public SettingsViewModel(IUploadedFileStorage uploadedFileStorage, FileStorageHelper fileStorageHelper, UserService userService)
        {
            this.uploadedFileStorage = uploadedFileStorage;
            this.fileStorageHelper = fileStorageHelper;
            this.userService = userService;
        }


        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                Data = userService.GetUserInfo(GetUserId());
            }

            return base.PreRender();
        }

        /// <summary>
        /// Gets the file from the temporary storage, stores it in the Images folder and updates the Data.ImageUrl property. 
        /// The changes will be stored in the database when the user saves the form.
        /// </summary>
        public void ProcessFile()
        {
            if (AvatarFiles.Files.Any())
            {
                // TODO: the image should be resized to some reasonable dimensions

                // save the file in the Images folder and update the ImageUrl property
                var stream = uploadedFileStorage.GetFile(AvatarFiles.Files[0].FileId);
                Data.ImageUrl = fileStorageHelper.StoreFile(stream, AvatarFiles.Files[0].FileName);

                // delete temporary file and clear the upload control collection
                uploadedFileStorage.DeleteFile(AvatarFiles.Files[0].FileId);
                AvatarFiles.Clear();
            }
        }

        /// <summary>
        /// Saves the user profile.
        /// </summary>
        public void Save()
        {
            try
            {
                userService.UpdateUserInfo(Data, GetUserId());
                AlertType = "success";
                AlertText = "Your profile was updated.";
            }
            catch (Exception ex)
            {
                AlertType = "danger";
                AlertText = ex.Message;
            }
        }
    }
}

