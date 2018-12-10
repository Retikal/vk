using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VkNet.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private async void AuthButton_Click(object sender, RoutedEventArgs e)
		{

			string token = string.Empty;
			if(ClientId.Text == string.Empty)
			{
				var message = new Windows.UI.Popups.MessageDialog("Вы не указали Id Вашего приложения", "Ошибка");
				await message.ShowAsync();
			}else
			{
				var startUri = new Uri($"https://oauth.vk.com/authorize?client_id={ClientId.Text}&display=popup&redirect_uri=https://oauth.vk.com/blank.html&scope={Scope.Text}&response_type=token&v=5.80&state=123456");
				var endUri = new Uri("https://oauth.vk.com/blank.html");

				//Авторизация в Popup окне с использованием стандартного windows web authentication broker
				try
				{
					var result = await WebAuthenticationBroker
											.AuthenticateAsync(
											WebAuthenticationOptions.None,
											startUri,
											endUri);

					switch (result.ResponseStatus)
					{
						case WebAuthenticationStatus.Success:
							var responseString = result.ResponseData;
							char[] separators = { '=', '&' };
							string[] responseContent = responseString.Split(separators);
							string accessToken = responseContent[1];
							long userId = long.Parse(responseContent[5]);
							token = accessToken;
							break;
						case WebAuthenticationStatus.ErrorHttp:
							var message = new Windows.UI.Popups
								.MessageDialog("Проверте Ваше подключение к Интернету.", "Нет доступа к интернету");
							await message.ShowAsync();
							return;
						case WebAuthenticationStatus.UserCancel:
							return;
					}

					//Вывод токена в текстовое поле.
					UserToken.Text = $"Ваш токен: {token}";
					UserToken.Visibility = Visibility.Visible;

					//Дальше можно использовать этот токен в VkNet:

					//VkApi api = new VkApi();
					//api.Authorize(new ApiAuthParams { AccessToken = "токен полученый в процессе авторизации"});
					//и можно смело использовать api для всех нужных запросов.


					//Скрытие ненужных элементов интерфейса.
					AuthButton.Visibility = Visibility.Collapsed;
					ClientId.Visibility = Visibility.Collapsed;
				}
				catch (Exception ex)
				{
					var message = new Windows.UI.Popups.MessageDialog(ex.ToString(), "Необработанное исключение");
					await message.ShowAsync();
					return;
				}
			}
		}
	}
}
