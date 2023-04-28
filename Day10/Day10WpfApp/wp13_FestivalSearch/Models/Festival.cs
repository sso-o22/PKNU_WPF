using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp13_FestivalSearch.Models
{
    public class Festival
    {
        /*
         "UC_SEQ": 253, (콘텐츠ID)
        "MAIN_TITLE": "수국축제(한,영,중간,중번,일)",
        "GUGUN_NM": "영도구",
        "LAT": 35.05602,
        "LNG": 129.08812,
        "PLACE": "태종대, 태종사",
        "TITLE": "태종대에서 만난 오색찬란 수국의 매력",
        "SUBTITLE": "부산 수국꽃문화축제를 가다",
        "MAIN_PLACE": "태종대, 태종사",
        "ADDR1": "부산광역시 영도구 전망로 119",
        "ADDR2": "",
        "CNTCT_TEL": "051-405-2727",
        "HOMEPAGE_URL": "",
        "TRFC_INFO": "버스 186, 30, 66, 8, 88, 101, 1006 태종대(태종대온천) 하차\n주차 태종대유원지 주차장 (유료)",
        "USAGE_DAY": "축제 매년 6월 말 ~ 7월 초",
        "USAGE_DAY_WEEK_AND_TIME": "태종대유원지 개방시간 하절기(3월~10월) 04:00~24:00 / 동절기(11월~2월) 05:00~24:00",
        "USAGE_AMOUNT": "무료",
        "MAIN_IMG_NORMAL": "https://www.visitbusan.net/uploadImgs/files/cntnts/20191222160520749_ttiel",
        "MAIN_IMG_THUMB": "https://www.visitbusan.net/uploadImgs/files/cntnts/20191222160520749_thumbL",
        "ITEMCNTNTS": "본격적인 휴가철이 시작되기 전, 초여름의 부산에서 반드시 만나야할 축제가 있다.
         */

        //public int Id { get; set; }
        public int Uc_Seq { get; set; }  // key
        public string Main_Title { get; set; }
        public string Gugun_Nm { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Place { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }   
        public string Main_Place { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Cntct_Tel { get; set; }
        public string Homepage_Url { get; set; }
        public string Trfc_Info {  get; set; }
        public string Usage_Day { get; set; }
        public string Usage_Day_Week_And_Time { get; set;}
        public string Usage_Amount { get; set; }
        public string Main_Img_Normal { get; set; }
        public string Main_Img_Thumb { get; set; }
        public string Itemcntnts { get; set; }



    }
}
