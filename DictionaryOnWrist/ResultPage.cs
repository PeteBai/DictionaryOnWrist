using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace DictionaryOnWrist
{
    class ResultPage : Tizen.Wearable.CircularUI.Forms.CirclePage
    {
        public ResultPage(string disp)
        {
            Button why = new Button()
            {
                Text = "为什么没有详细解释？",
                FontSize = 5,
                BackgroundColor = Color.Transparent,
                FontAttributes = FontAttributes.Italic,
            };
            why.Clicked += whyBtnClickedFunc;
            String display = disp;
            int resLen = disp.Length;
            int fontSz = 16;
            if (resLen >= 7 && resLen <= 10)
                fontSz = 10;
            else if(resLen > 10)
            {
                display = "查询内容过长";
            }
            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new Label(){Text = display, HorizontalTextAlignment=TextAlignment.Center,FontSize=fontSz},
                    new Label(){Text=" "},
                    new Label()
                    {
                        Text = "由百度翻译提供", 
                        HorizontalTextAlignment=TextAlignment.Center,
                        FontSize = 5,
                    },
                    why,
                },
            };
        }
        private void whyBtnClickedFunc(Object sender, EventArgs e)
        {
            Toast.DisplayText("我是个人开发者，是个学生，而几乎所有的网络词典接口都要收费，因此无法提供诸如音标等更详细的解析。");
        }
    }
}
