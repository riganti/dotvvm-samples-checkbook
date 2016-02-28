using System;
using DotVVM.Framework.Runtime.Filters;
using System.ComponentModel.DataAnnotations;
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
        public UserInfoData Data { get; set; }

        public string AlertText { get; set; }

        public string AlertType { get; set; }


        // To make file uploads work you have to register the IUploadedFilesStorage in the Startup.cs file
        public UploadedFilesCollection AvatarFiles { get; set; } = new UploadedFilesCollection();


        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                Data = UserService.GetUserInfo(GetUserId());
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
                var storage = Context.Configuration.ServiceLocator.GetService<IUploadedFileStorage>();
                var stream = storage.GetFile(AvatarFiles.Files[0].FileId);
                Data.ImageUrl = FileStorageHelper.StoreFile(stream, AvatarFiles.Files[0].FileName);

                // delete temporary file and clear the upload control collection
                storage.DeleteFile(AvatarFiles.Files[0].FileId);
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
                UserService.UpdateUserInfo(Data, GetUserId());
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

