using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;

namespace wp08_personalInfoApp.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        // View에서 사용할 멤버변수 선언
        // 입력쪽 변수
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate;

        // 결과 출력쪽 변수
        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate;  // 생일날짜 출력할 때는 문자열 대체
        private string outAdult;
        private string outBirthDay;
        private string outZodiac;

        // 일이 많아짐. 실제로 사용할 속성
        // 입력을 위한 속성들
        public string InFirstName 
        { 
            get => inFirstName; 
            set
            {
                inFirstName = value;
                RaisePropertyChanged(nameof(InFirstName));  // "InFirstName"
            }
        }

        public string InLastName 
        { 
            get => inLastName; 
            set
            {
                InLastName = value;
                RaisePropertyChanged(nameof(InLastName));
            }
        }

        public string InEmail 
        { 
            get => inEmail; 
            set
            {
                InEmail = value;
                RaisePropertyChanged(nameof(InEmail));
            }
        }
        public DateTime InDate 
        { 
            get => inDate; 
            set
            {
                InDate = value;
                RaisePropertyChanged(nameof(InDate));
            }
        }

        // 출력을 위한 속성들
        public string OutFirstName 
        { 
            get => outFirstName; 
            set
            {
                OutFirstName = value;
                RaisePropertyChanged(nameof(OutFirstName));
            }
        }
        public string OutLastName 
        { 
            get => outLastName; 
            set
            {
                OutLastName = value;
                RaisePropertyChanged(nameof(OutLastName));
            }
        }
        public string OutEmail 
        { 
            get => outEmail; 
            set
            {
                OutEmail = value;
                RaisePropertyChanged(nameof(OutEmail));
            }
        }
        public string OutDate 
        { 
            get => outDate; 
            set
            {
                OutDate = value;
                RaisePropertyChanged(nameof(OutDate));
            }
        }
        public string OutAdult 
        { 
            get => outAdult; 
            set
            {
                OutAdult = value;
                RaisePropertyChanged(nameof(OutAdult));
            }
        }
        public string OutBirthDay 
        { 
            get => outBirthDay; 
            set
            {
                OutBirthDay = value;
                RaisePropertyChanged(nameof(OutBirthDay));
            }
        }
        public string OutZodiac 
        { 
            get => outZodiac; 
            set
            {
                OutZodiac = value;
                RaisePropertyChanged(nameof(OutZodiac));
            }
        }
    }
}
