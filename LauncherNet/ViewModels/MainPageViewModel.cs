using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.Notifications;
using LauncherNet.Models;
using LauncherNet.Views;
using ReactiveUI;

namespace LauncherNet.ViewModels;

public class MainPageViewModel(MainWindow window) : ViewModelBase(window)
{
    public ICommand LoginCommand => ReactiveCommand.CreateFromTask(TryLoginAsync);


    private async Task TryLoginAsync()
    {
        var info = await GetAsync();
        if (info != null)
        {
            
        }
        else
        {
            // TODO: Show a messagebox to the user warning him that the Microsoft Authentication Service might be unavailable
        }
        

        // try
        // {
        //     var client = new HttpClient();
        //     var model = new AuthModel();
        //     var data = JsonSerializer.Serialize(model);
        //     var content = new StringContent(data, Encoding.UTF8, "application/json");
        //     var response = await client.PostAsync("https://authserver.mojang.com/authenticate", content);
        //     var json = await response.Content.ReadAsStringAsync();
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        //     throw;
        // }

    }

    private async Task<Dictionary<string, string>?> GetAsync()
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync(
                "https://login.live.com/oauth20_authorize.srf?client_id=000000004C12AE6F&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=service::user.auth.xboxlive.com::MBI_SSL&display=touch&response_type=token&locale=en");

            var content = await response.Content.ReadAsStringAsync();

            var index = content.IndexOf("urlPost:", StringComparison.Ordinal) + "urlPost:".Length;
            int startQuoteIndex = content.IndexOf("'", index, StringComparison.Ordinal);
            int endQuoteIndex = content.IndexOf("'", startQuoteIndex + 1, StringComparison.Ordinal);
            var urlPost = content.Substring(startQuoteIndex + 1, endQuoteIndex - startQuoteIndex - 1);
            
            var info = new Dictionary<string, string>();
            info.Add("urlPost", urlPost);

            var startIndex = content.IndexOf("sFTTag:'<input type=\"hidden\" name=\"PPFT\" id=\"i0327\" value=\"", StringComparison.Ordinal);
            var endIndex = content.IndexOf("/>", index + 1, StringComparison.Ordinal);
            var sftTag = content.Substring(startIndex + 1, endIndex - startIndex - 1);
            
            info.Add("sftTag", sftTag);
            
            return info;
        }
        catch (Exception e)
        {
            // TODO: Show a messagebox to the user with the relative error message
        }

        return null;
    }
}