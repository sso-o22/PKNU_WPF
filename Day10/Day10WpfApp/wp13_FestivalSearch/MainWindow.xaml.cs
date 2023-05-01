using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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

        // 조회
        private async void BtnSearchFstiv_Click(object sender, RoutedEventArgs e)
        {
            string apiKey = "hoTBwQLPqblAry8FHGON1eWrxGhlTnfaVQ8U2GxfjrF7jx01qHlVXtBts1GfhsXTphn4yrYM6eeCX3iB7aq0dQ%3D%3D";
            string openApiUri = $"https://apis.data.go.kr/6260000/FestivalService/getFestivalKr?serviceKey={apiKey}&pageNo=1&numOfRows=32&resultType=json";
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
                            Middle_Size_Rm1 = Convert.ToString(val["MIDDLE_SIZE_RM1"])    
                        });
                    }
                    this.DataContext = festivals;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {festivals.Count}건 조회완료";
                    CboRegion.Text = null;
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }


        // 검색한 결과 DB(MySQL)에 저장
        private async void BtnSaveFstiv_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "축제를 선택하세요.");
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
                                            Middle_Size_Rm1)
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
                                            @Middle_Size_Rm1)";
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
                            cmd.Parameters.AddWithValue("@Middle_Size_Rm1", item.Middle_Size_Rm1);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }
                    await Commons.ShowMessageAsync("저장", "DB 저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }
            }
            catch (Exception)
            {
                await Commons.ShowMessageAsync("오류", "이미 저장된 데이터입니다.");
            }
        }


        // 셀 클릭하면 이미지, 교통편 or 지도(위도, 경도로) 띄우기
        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                string imagepath = string.Empty;
                string txttrfc = string.Empty;

                if (GrdResult.SelectedItem is Festival)  // openAPI로 검색된 축제의 사진, 교통정보
                {
                    var item = GrdResult.SelectedItem as Festival;
                    imagepath = item.Main_Img_Normal;
                    txttrfc = item.Trfc_Info;
                }

                Debug.WriteLine(imagepath);
                if (string.IsNullOrEmpty(imagepath))  // 이미지가 없으면 No_Picture, 교통정보 없으면 비우기
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                    TxtTrfcInfo.Text = null;
                }
                else  // 이미지 경로, 교통정보가 있으면
                {
                    ImgPoster.Source = new BitmapImage(new Uri($"{imagepath}", UriKind.RelativeOrAbsolute));
                    TxtTrfcInfo.Text = txttrfc;
                }
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", "이미지로드 오류발생");
            }
        }

        // 구군으로 콤보박스
        private void CboRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboRegion.SelectedValue != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Uc_Seq,
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
                                         Middle_Size_Rm1
                                    FROM festival
                                   WHERE Gugun_Nm = @Gugun_Nm";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("Gugun_Nm", CboRegion.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "festival");
                    List<Festival> item = new List<Festival>();
                    foreach (DataRow row in ds.Tables["festival"].Rows)
                    {
                        item.Add(new Festival
                        {
                            Uc_Seq = Convert.ToInt32(row["UC_SEQ"]),
                            Main_Title = Convert.ToString(row["MAIN_TITLE"]),
                            Gugun_Nm = Convert.ToString(row["GUGUN_NM"]),
                            Lat = Convert.ToDouble(row["LAT"]),
                            Lng = Convert.ToDouble(row["LNG"]),
                            Place = Convert.ToString(row["PLACE"]),
                            Title = Convert.ToString(row["TITLE"]),
                            Subtitle = Convert.ToString(row["SUBTITLE"]),
                            Main_Place = Convert.ToString(row["MAIN_PLACE"]),
                            Addr1 = Convert.ToString(row["ADDR1"]),
                            Addr2 = Convert.ToString(row["ADDR2"]),
                            Cntct_Tel = Convert.ToString(row["CNTCT_TEL"]),
                            Homepage_Url = Convert.ToString(row["HOMEPAGE_URL"]),
                            Trfc_Info = Convert.ToString(row["TRFC_INFO"]),
                            Usage_Day = Convert.ToString(row["USAGE_DAY"]),
                            Usage_Day_Week_And_Time = Convert.ToString(row["USAGE_DAY_WEEK_AND_TIME"]),
                            Usage_Amount = Convert.ToString(row["USAGE_AMOUNT"]),
                            Main_Img_Normal = Convert.ToString(row["MAIN_IMG_NORMAL"]),
                            Main_Img_Thumb = Convert.ToString(row["MAIN_IMG_THUMB"]),
                            Middle_Size_Rm1 = Convert.ToString(row["MIDDLE_SIZE_RM1"])
                        });
                    }

                    this.DataContext = item;
                    StsResult.Content = $"OpenAPI {item.Count}건 조회완료";
                }
            }

            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB조회 클리어";
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT Gugun_Nm
                                FROM festival
                               GROUP BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Gugun_Nm"]));
                }
                CboRegion.ItemsSource = saveDateList;
            }
        }

        // 버튼 클릭하면 지도 링크 새창으로 열기
        private async void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 눌러주세요.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "축제를 선택하세요.");
                return;
            }
            else
            {
                var selItem = GrdResult.SelectedItem as Festival;

                var mapWindow = new MapWindow(selItem.Lat, selItem.Lng);  // 부모창 위치값을 자식창으로 전달
                mapWindow.Owner = this;  // MainWindow 부모
                mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;  // 부모창 중간에 출력
                mapWindow.ShowDialog();
            }
        }

        // 버튼 클릭하면 축제 링크 새창으로 열기
        private async void BtnLink_Click(object sender, RoutedEventArgs e)
        {
            var selItem = GrdResult.SelectedItem as Festival;
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 눌러주세요.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "축제를 선택하세요.");
                return;
            }

            if (string.IsNullOrEmpty(selItem.Homepage_Url))  // 링크가 없으면
            {
                await Commons.ShowMessageAsync("오류", "홈페이지 링크가 없습니다.");
            }
            else  // 홈페이지 링크가 있으면
            {
                var mapWindow = new LinkWindow(selItem.Homepage_Url);  // 부모창 위치값을 자식창으로 전달
                mapWindow.Owner = this;  // MainWindow 부모
                mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;  // 부모창 중간에 출력
                mapWindow.ShowDialog();
            }                
        }
    }
}