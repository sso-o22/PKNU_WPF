using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using wp13_FestivalSearch.Logics;
using wp13_FestivalSearch.Models;

namespace wp13_FestivalSearch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnSearchFstiv_Click(object sender, RoutedEventArgs e)
        {
            string apiKey = "hoTBwQLPqblAry8FHGON1eWrxGhlTnfaVQ8U2GxfjrF7jx01qHlVXtBts1GfhsXTphn4yrYM6eeCX3iB7aq0dQ%3D%3D";
            string openApiUri = $"https://apis.data.go.kr/6260000/FestivalService/getFestivalKr?serviceKey={apiKey}&pageNo=1&numOfRows=10&resultType=json";
            string result = string.Empty;  // 결과값

            // API 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);  // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync();  // 요청한 결과를 응답에 할당 / 비동기로 동작하는 메서드
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();  // json결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            //var status = Convert.ToInt32(jsonResult["status"]);
            var code = Convert.ToString(jsonResult["getFestivalKr"]["header"]["code"]);

            try
            {
                if (code == "00")  // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["getFestivalKr"]["item"];
                    var json_array = data as JArray;

                    var festivals = new List<Festival>();  // json에서 넘어온 배열을 담을 장소
                    foreach (var val in json_array)
                    {
                        festivals.Add(new Festival
                        {
                            Uc_Seq = Convert.ToInt32(val["UC_SEQ"]),
                            Main_Title = Convert.ToString(val["MAIN_TITLE"]),
                            Gugun_Nm = Convert.ToString(val["GUGUN_NM"]),
                            Lat = Convert.ToDouble(val["LAT"]),
                            Lng = Convert.ToDouble(val["LNG"]),
                            Place = Convert.ToString(val["PLACE"]),
                            Title = Convert.ToString(val["TITLE"]),
                            Subtitle = Convert.ToString(val["SUBTITLE"]),
                            Main_Place = Convert.ToString(val["MAIN_PLACE"]),
                            Addr1 = Convert.ToString(val["ADDR1"]),
                            Addr2 = Convert.ToString(val["ADDR2"]),
                            Cntct_Tel = Convert.ToString(val["CNTCT_TEL"]),
                            Homepage_Url = Convert.ToString(val["HOMEPAGE_URL"]),
                            Trfc_Info = Convert.ToString(val["TRFC_INFO"]),
                            Usage_Day = Convert.ToString(val["USAGE_DAY"]),
                            Usage_Day_Week_And_Time = Convert.ToString(val["USAGE_DAY_WEEK_AND_TIME"]),
                            Usage_Amount = Convert.ToString(val["USAGE_AMOUNT"]),
                            Main_Img_Normal = Convert.ToString(val["MAIN_IMG_NORMAL"]),
                            Main_Img_Thumb = Convert.ToString(val["MAIN_IMG_THUMB"]),
                            Itemcntnts = Convert.ToString(val["ITEMCNTNTS"])
                        });
                    }
                    this.DataContext = festivals;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {festivals.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }

        // 검색한 결과 DB(MySQL)에 저장
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO festival
                                            (Uc_Seq,
                                            Main_Title,
                                            Gugun_Nm,
                                            Lat,
                                            Lng,
                                            Place,
                                            Title,
                                            Subtitle,
                                            Main_Place,
                                            Addr1,
                                            Addr2,
                                            Cntct_Tel,
                                            Homepage_Url,
                                            Trfc_Info,
                                            Usage_Day,
                                            Usage_Day_Week_And_Time,
                                            Usage_Amount,
                                            Main_Img_Normal,
                                            Main_Img_Thumb,
                                            Itemcntnts)
                                            VALUES
                                            (@Uc_Seq,
                                            @Main_Title,
                                            @Gugun_Nm,
                                            @Lat,
                                            @Lng,
                                            @Place,
                                            @Title,
                                            @Subtitle,
                                            @Main_Place,
                                            @Addr1,
                                            @Addr2,
                                            @Cntct_Tel,
                                            @Homepage_Url,
                                            @Trfc_Info,
                                            @Usage_Day,
                                            @Usage_Day_Week_And_Time,
                                            @Usage_Amount,
                                            @Main_Img_Normal,
                                            @Main_Img_Thumb,
                                            @Itemcntnts)";
                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is Festival)
                        {
                            var item = temp as Festival;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Uc_Seq", item.Uc_Seq);
                            cmd.Parameters.AddWithValue("@Main_Title", item.Main_Title);
                            cmd.Parameters.AddWithValue("@Gugun_Nm", item.Gugun_Nm);
                            cmd.Parameters.AddWithValue("@Lat", item.Lat);
                            cmd.Parameters.AddWithValue("@Lng", item.Lng);
                            cmd.Parameters.AddWithValue("@Place", item.Place);
                            cmd.Parameters.AddWithValue("@Title", item.Title);
                            cmd.Parameters.AddWithValue("@Subtitle", item.Subtitle);
                            cmd.Parameters.AddWithValue("@Main_Place", item.Main_Place);
                            cmd.Parameters.AddWithValue("@Addr1", item.Addr1);
                            cmd.Parameters.AddWithValue("@Addr2", item.Addr2);
                            cmd.Parameters.AddWithValue("@Cntct_Tel", item.Cntct_Tel);
                            cmd.Parameters.AddWithValue("@Homepage_Url", item.Homepage_Url);
                            cmd.Parameters.AddWithValue("@Trfc_Info", item.Trfc_Info);
                            cmd.Parameters.AddWithValue("@Usage_Day", item.Usage_Day);
                            cmd.Parameters.AddWithValue("@Usage_Day_Week_And_Time", item.Usage_Day_Week_And_Time);
                            cmd.Parameters.AddWithValue("@Usage_Amount", item.Usage_Amount);
                            cmd.Parameters.AddWithValue("@Main_Img_Normal", item.Main_Img_Normal);
                            cmd.Parameters.AddWithValue("@Main_Img_Thumb", item.Main_Img_Thumb);
                            cmd.Parameters.AddWithValue("@Itemcntnts", item.Itemcntnts);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", $"DB 저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류 {ex.Message}");
            }
        }

    }
}