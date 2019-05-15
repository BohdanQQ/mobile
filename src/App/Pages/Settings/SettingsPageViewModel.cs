﻿using Bit.App.Resources;
using Bit.Core.Abstractions;
using Bit.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bit.App.Pages
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly IPlatformUtilsService _platformUtilsService;
        private readonly ICryptoService _cryptoService;
        private readonly IUserService _userService;

        public SettingsPageViewModel()
        {
            _platformUtilsService = ServiceContainer.Resolve<IPlatformUtilsService>("platformUtilsService");
            _cryptoService = ServiceContainer.Resolve<ICryptoService>("cryptoService");
            _userService = ServiceContainer.Resolve<IUserService>("userService");

            PageTitle = AppResources.Settings;
            BuildList();
        }

        public List<SettingsPageListGroup> GroupedItems { get; set; }

        public async Task AboutAsync()
        {
            var debugText = string.Format("{0}: {1}", AppResources.Version,
                _platformUtilsService.GetApplicationVersion());
            var text = string.Format("© 8bit Solutions LLC 2015-{0}\n\n{1}", DateTime.Now.Year, debugText);
            var copy = await _platformUtilsService.ShowDialogAsync(text, AppResources.Bitwarden, AppResources.Copy,
                AppResources.Close);
            if(copy)
            {
                await _platformUtilsService.CopyToClipboardAsync(debugText);
            }
        }

        public void Help()
        {
            _platformUtilsService.LaunchUri("https://help.bitwarden.com/");
        }

        public async Task FingerprintAsync()
        {
            var fingerprint = await _cryptoService.GetFingerprintAsync(await _userService.GetUserIdAsync());
            var phrase = string.Join("-", fingerprint);
            var text = string.Format("{0}\n\n{1}", AppResources.YourAccountsFingerprint, phrase);
            var learnMore = await _platformUtilsService.ShowDialogAsync(text, AppResources.FingerprintPhrase,
                AppResources.LearnMore, AppResources.Close);
            if(learnMore)
            {
                _platformUtilsService.LaunchUri("https://help.bitwarden.com/article/fingerprint-phrase/");
            }
        }

        private void BuildList()
        {
            var doUpper = Device.RuntimePlatform != Device.Android;
            var manageItems = new List<SettingsPageListItem>
            {
                new SettingsPageListItem { Name = AppResources.Folders },
                new SettingsPageListItem { Name = AppResources.Sync }
            };
            var securityItems = new List<SettingsPageListItem>
            {
                new SettingsPageListItem { Name = AppResources.LockOptions },
                new SettingsPageListItem { Name = string.Format(AppResources.UnlockWith, AppResources.Fingerprint) },
                new SettingsPageListItem { Name = AppResources.UnlockWithPIN },
                new SettingsPageListItem { Name = AppResources.Lock },
                new SettingsPageListItem { Name = AppResources.TwoStepLogin }
            };
            var accountItems = new List<SettingsPageListItem>
            {
                new SettingsPageListItem { Name = AppResources.ChangeMasterPassword },
                new SettingsPageListItem { Name = AppResources.FingerprintPhrase },
                new SettingsPageListItem { Name = AppResources.LogOut }
            };
            var otherItems = new List<SettingsPageListItem>
            {
                new SettingsPageListItem { Name = AppResources.Options },
                new SettingsPageListItem { Name = AppResources.About },
                new SettingsPageListItem { Name = AppResources.HelpAndFeedback },
                new SettingsPageListItem { Name = AppResources.RateTheApp }
            };
            GroupedItems = new List<SettingsPageListGroup>
            {
                new SettingsPageListGroup(manageItems, AppResources.Manage, doUpper),
                new SettingsPageListGroup(securityItems, AppResources.Security, doUpper),
                new SettingsPageListGroup(accountItems, AppResources.Account, doUpper),
                new SettingsPageListGroup(otherItems, AppResources.Other, doUpper)
            };
        }
    }
}