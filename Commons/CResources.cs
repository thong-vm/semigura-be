namespace semigura.Commons
{
    public class CResources
    {
        public static string S01001_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S01002.username);
            }
        }

        public static string S01001_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S01002.password);
            }
        }

        //--S02001--------------------------------------------
        public static string S02001_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.username);
            }
        }

        public static string S02001_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.password);
            }
        }

        public static string S02001_03
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.email);
            }
        }
        public static string S02001_04
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.baume1);
            }
        }
        public static string S02001_05
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.alcohol);
            }
        }
        public static string S02001_06
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.acid);
            }
        }
        public static string S02001_07
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.amomiAcid);
            }
        }
        public static string S02001_08
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S02001.dateTime);
            }
        }

        //--S03001--------------------------------------------
        public static string S03001_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S03002.factory);
            }
        }

        public static string S03001_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S03002.location);
            }
        }

        public static string S03001_03
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S03002.lot);
            }
        }

        //--S03002--------------------------------------------
        public static string S03002_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S03002.location);
            }
        }

        //--S09001--------------------------------------------
        public static string S09001_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.material);
            }
        }

        public static string S09001_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.rice_polishing_ratio_name);
            }
        }
        public static string S09001_03
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.type);
            }
        }

        public static string S09001_04
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.rice_polishing_ratio_value);
            }
        }

        public static string S09001_05
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.temp_min);
            }
        }

        public static string S09001_06
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09001.temp_max);
            }
        }

        //--S09002--------------------------------------------
        public static string S09002_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09002.email);
            }
        }

        //--S09003--------------------------------------------
        public static string S09003_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09003.tankcode);
            }
        }

        public static string S09003_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01005, Resources.S09003.factory);
            }
        }

        public static string S09003_03
        {
            get
            {
                return string.Format(Resources.SharedResource.C01005, Resources.S09003.location);
            }
        }
        //--S09004--------------------------------------------
        public static string S09004_01
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09004.terminal_code);
            }
        }

        public static string S09004_02
        {
            get
            {
                return string.Format(Resources.SharedResource.C01001, Resources.S09004.type);
            }
        }
        //Rank check
        public static string RankCheck
        {
            get
            {
                return string.Format(Resources.SharedResource.format, Resources.SharedResource.rankCheck);
            }
        }
        // Length Check
        public static string LengthCheck
        {
            get
            {
                return string.Format(Resources.SharedResource.format, Resources.SharedResource.lengthCheck);
            }
        }

    }
}