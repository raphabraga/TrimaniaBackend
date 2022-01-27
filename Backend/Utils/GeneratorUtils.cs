using System;
using System.Text;

namespace Backend.Utils
{
    public static class GeneratorUtils
    {
        public static string GenerateNumber(int minDecimals, int maxDecimals = 0)
        {
            var rand = new Random();
            if (maxDecimals == 0)
                return rand.Next((int)(Math.Pow(10.0f, minDecimals - 1)), (int)Math.Pow(10.0f, minDecimals)).ToString();
            else
                return rand.Next((int)(Math.Pow(10.0f, minDecimals - 1)), (int)Math.Pow(10.0f, maxDecimals)).ToString();
        }

        public static string GeneratePassword()
        {
            return GenerateNumber(6) + "#Q";
        }

        public static string GenerateAttribute(string attribute, int idx = -1)
        {
            string[] attr;
            var cities = new[] { "Iyuledo", "Hamore", "Brivine", "Vleburg", "Frehrith", "Qrolk", "Zrester", "Louis", "Infield", "Osabert", "Raxbus", "Chutin", "Fruledo", "Frehtol", "Klepolis", "Qeah", "Trison", "Ozhinas", "Ouverwood", "Eryledo", "Kroshire", "Ylodence", "Xicbury", "Dosphis", "Khuland", "Phego", "Phose", "Saka", "Andsea", "Ontsall", "Wupolis", "Kriefshire", "Vristin", "Jevine", "Cralland", "Axury", "Heah", "Goit", "Orkset", "Osacaster", "Qribury", "Piltol", "Fragan", "Slieshire", "Wrodset", "Yrarc", "Srento", "Phido", "Iriecester", "Iasey", "Kluokdon", "Fedon", "Chuopolis", "Khapcaster", "Ziengend", "Ylance", "Zlento", "Qreigh", "Oriagend", "Ontstin", "Tibury", "Wreakford", "Prayross", "Busa", "Blufvale", "Xouis", "Yhirie", "Zrurgh", "Acoledo", "Athestin", "Frumond", "Radshire", "Qidgan", "Nuprora", "Fluafast", "Tam", "Zlane", "Asolk", "Illepolis", "Athewood", "Streyrough", "Pharding", "Esupool", "Kritin", "Houfast", "Mork", "Plolk", "Upido", "Oithull", "Arctol", "Ziasa", "Yhoxson", "Chaxbridge", "Kheford", "Zomrough", "Malo", "Rury", "Geah", "Osastead", "Irieburn" };

            var names = new[] { "Frederick Hayes", "Harvey Fisher", "Zachary Price", "Jack Jordan", "Louis Owen", "Emery Sharpe", "Johan Ferguson", "Graeme Manning", "Rex Ballard", "Markus Mcdowell", "Holly Evans", "Lydia Jones", "Morgan Read", "Isabelle Cox", "Eloise Fletcher", "Jolene Gentry", "Lilianna Macias", "Rosalie Mcclain", "Catalina Donovan", "Miranda Burnett", "Christopher Clarke", "Jack Parker", "Harrison Dawson", "Owen Carter", "Kayden Holland", "Howard Hammond", "Cameron Sanford", "Bentley Mcgee", "Darien Powers", "Malik Eaton", "Sofia Kaur", "Kate Williams", "Alex Jones", "Elsie Gardner", "Harriet Simpson", "Nyla Wood", "Luciana Wheeler", "Cristina Long", "Linda Rogers", "Harmony Wiggins", "Michael George", "Jack Bates", "Spencer Wilkinson", "Dexter Spencer", "Rhys Patel", "Dominik Carpenter", "Thaddeus Mcguire", "Nehemiah Knox", "Rhys Briggs", "Ryker Curry", "Paige Sutton", "Alice Hunter", "Ellie Adams", "Cerys Matthews", "Mia Lawrence", "Coraline Ingram", "Kennedy Schneider", "Eloise Moody", "Courtney Ashley", "Danica Nieves", "Jude Murray", "Zak Fraser", "Scott Simpson", "John White", "Kayden Watts", "Jaxon Pickett", "Albert Foster", "Bryant Ramirez", "Beau Rojas", "Ronin Spencer", "Erin Atkinson", "Rose Moore", "Paige Holmes", "Jennifer Porter", "Brooke Andrews", "Olivia Britt", "Leila Martin", "Scarlet Preston", "Nathalie Gonzales", "Giana Bishop", "Evan Roberts", "Kai Atkinson", "Cody Webb", "Leo Williamson", "Harry Poole", "Holden Kelly", "Jamar Hernandez", "Tyler Odom", "Alfred Ruiz", "Neil Russell", "Amber Hunt", "Mia Robinson", "Mollie Burke", "Anna Johnston", "Jade Mills", "Maritza Summers", "Jayleen Waters", "Megan Yates", "Faith Adams", "Irene Garrett" };

            var neighbourhoods = new[] { "Wog Yard", "Waterside What", "West Scelmim", "Upper West Qamrim", "Lower South Woreoct", "Lower North Gidud", "Blerrag Corner", "Quppuld South", "Glessewlul Place", "Scrucirlork Row", "Waterside Gund", "Slup Park", "Upper South Thrakwunk", "Farnud Square", "South Skumul", "Frilad East", "Tuggerk Center", "Lower North Segift", "Sciohigalp", "Pisiolgab East", "Upper Leod", "Lower East Snub", "Little Strilract", "Fastoos Valley", "Downtown Faggild", "Nutulp Hills", "Upper South Primmamp", "Plegoolf Square", "Skooppatnild Grove", "Hugeoldeom Square", "Lower East Plet", "Driolt Grove", "Yutwent Cross", "Geotbuc Square", "Upper South Vaisiol", "Lower Hirrest", "Flimek Garden", "Lower North Sinnig", "Yeneakwon Bazaar", "Berikwurd Side", "Gremp Heights", "Crond Hill", "Little Glactat", "Upper South Foxoc", "Upper South Tannuc", "Tetond Hills", "North Fonnoamp", "Deaffob West", "Noosoolwas East", "West Pattairlict Corner", "Sleap Vale", "Zam Place", "Whiownoag Cross", "Jorgeot West", "Upper West Shrecom", "Smedek Grove", "Upper West Drapulf", "Peammosp Bazaar", "Qiommoarleg Point", "Throteofon Park", "Hob Point", "Nic East", "Midtown Rofib", "Downtown Huwnoc", "Thruneb Garden", "Upper Sprohurd", "Upper Poggul", "Upper Friaffusp", "Beffengailp South", "Yimmoruld Boulevard", "Little Birk", "Midtown Daib", "Xiastrot Street", "Waterside Brirthoonk", "Wrettild Town", "Upper West Noagim", "Hisob Circle", "Lower South Schaimunt", "Ramartrerk Acre", "Mipoobreb Circle", "Lower Fisp ", "Yeld Acre", "Rulas Valley", "Richeeb Point", "Lower West Smiffesp ", "Getunt Town", "Qulen Place", "Twuppost Street", "Cammoveost Square", "Reasacteec Town", "Misk Street", "Downtown Dreal ", "Swurmuft Street", "Lower South Doldop ", "Upper West Fettag ", "Upper North Blideg ", "Dipisk Plaza", "Midtown Culoct ", "East Haiwurstos Cross", "Gliriceg Acre" };

            var products = new[] { "Pioneer Audio Amplifier", "Aiyima Audio Amplifier", "Sony 4K Television", "LG 4K Television", "Samsung 4K Television", "Xiaomi Vacuum Cleaner", "Bissell Vacuum Cleaner", "JBL Audio Speaker", "Xiaomi Audio Speaker", "Edifier Audio Speaker", "Cassio Desktop Calculator", "Sony Video Camera", "Canon Digital Photo Camera", "Nikon Digital Photo Camera", "Iphone Phone Charger", "Samsung Phone Charger", "Mondial Juice Centrifuge", "Weber Electric Barbecue Grill", "Macbook Laptop", "Asus Laptop", "Dell Laptop", "Samsung Laptop", "Xbox Game Console", "Playstation Game Console", "Nintendo Switch Game Console", "LG Remote Control", "Sony Remote Control", "Dell Personal Computers", "Dell Dock Station", "HP Dock Station", "LG DVD Player", "Sony BlueRay Player", "Panasonic BlueRay Player", "Kindle E-readers", "Mondial Fruit Mixer", "Seagate External Hard Drive (HDD)", "Toshiba External Hard Drive (HDD)", "Seagate External Optical Drive (ODD)", "Soundcore Headphone", "JBL Headphone", "Sony Headphone", "Edifier Headphone", "Philips AirFryer", "Sony Audio Recorder", "Sony Video Recorder", "Philips Home Theater", "Epson Deskjet Ink Printer", "HP Deskjet Ink Printer", "Xiaomi External Cell Phone Lense", "Mondial Liquidizers", "Weber Bread Machine", "Black&Decker Mini Electric Oven", "Black&Decker Mini Processor", "Motorola Modems", "Nespresso Coffee machines", "LavazzaCoffee machines", "Benq 4K Monitor", "LG 4K Monitor", "Razer Wireless Mouse", "Logitec Wireless Mouse", "Acer Netbook", "ZiYan 3d Glasses", "Presto Electric Pressure Cooker", "Revlon Hair Straightener", "Hamilton Food Processor", "Epson Video Projector", "Auking Video Projector", "AGPTek Media Player", "TP-Link Router", "Netgear Router", "Cisco Router", "Oster Toaster", "HP Scanner", "Epson Scanner", "Philips Hair Dryer", "IPad Tablet", "Samsung Tablet", "Logitec Wireless Keyboard", "Razer Wired Keyboard", "Samsung Mobile Phone", "Xiaomi Mobile Phone", "Panasonic Cordless Phone", "Sony Bluetooth Headset", "Razer Bluetooth mouse", "Mallory Table Fan", "HoneyWell Table Fan", "Rayban Sun glasses", "Takamine Acoustic Guitar", "Yamaha Acoustic Guitar", "Fender Electric Guitar", "Gibson Electric Guitar", "Xiaomi Smart Watch", "Apple Smart Watch", "Xiaomi Wrist Band", "Iphone Mobile Phone Cover", "Samsung Mobile Phone Cover", "Meross Smart Lamp", "Anycubic 3D Printer", "Sony VR Headset", "Hyper X Gaming Chair" };

            var states = new[] { "QS", "RK", "OT", "XK", "KY", "JH", "AZ", "BT", "OH", "JK", "WF", "CO", "IA", "NV", "CE", "IW", "ZI", "TY", "PZ", "SX", "SE", "CJ", "OL", "KC", "KG", "OY", "RJ", "UU", "ZY", "EB", "SV", "HN", "UO", "SQ", "AV", "XW", "IF", "WG", "VF", "PG", "SN", "VV", "FV", "KJ", "IO", "DO", "HZ", "OJ", "OB", "RQ", "FB", "XQ", "ON", "QT", "PS", "ZG", "HI", "TX", "HT", "KP", "DQ", "QO", "ZV", "AX", "KO", "SU", "UK", "GI", "SJ", "FN", "MA", "XT", "NN", "QU", "GB", "EY", "SW", "UP", "LP", "FT", "YB", "YY", "VO", "ZS", "EI", "GY", "WK", "IK", "IX", "LW", "WU", "XA", "SP", "NI", "IY", "ZU", "ZR", "VQ", "VB", "HU" };

            var streets = new[] { "Brookdale Fold", "Nicholas Glebe", "Parklands Top", "St John Cliff", "St Peter's Mead", "Kirk Pines", "Rosewood Ridings", "St James's Place", "Holme Ride", "Fifth Orchard", "Heathcote Estate", "Canal Wood", "Villa Moor", "Crossley Acres", "Pinewood Dene", "Chalfont Laurels", "Colchester Walk", "Reedmace", "Chandlers Close", "Crane Moorings", "Kent Dene", "Sandpiper Crescent", "Crescent Road", "Sandpiper Gardens", "Fox Town", "Cassillis Street", "Halstead Drove", "Dawson Maltings", "Rothesay Head", "Juniper Valley", "Boston Cottages", "Austin Springs", "Fitzroy Heights", "Lansdowne Town", "Holbrook Hawthorns", "Hollybush Glas", "Clinton Dell", "Cotswold Limes", "Causeway Village", "Burley Drive", "Markham Cloisters", "Nelson Ridge", "Lodge Bottom", "Willow Wood Close", "Cumberland Street", "Chancery Row", "County Houses", "Wilkinson Cedars", "Fleming Pleasant", "Owletts End", "Holmes Reach", "Manor Park Crest", "Belvoir Dale", "Hayfield Square", "South Lea", "Shawsdale Road", "Breydon Avenue", "Franklin Acres", "The Coronet", "Great Warren", "Burley Circus", "Lovel Avenue", "Bideford Wynd", "Higher Park Stenak", "Warleigh Road", "Holland Hey", "Jesmond Hollow", "Archer Circle", "Allhallowgate", "Kent Lane", "High Side", "Tydfil Road", "Howden Birches", "Downing Wharf", "Bideford Hollies", "Trevor Vale", "Eversley Park Road", "Belvoir Spinney", "Surrey Meadow", "Edwards Paddocks", "Addison Heights", "Mostyn Leas", "Saddlers South", "Malham Alley", "Farnham Ridge", "High Yard", "Harrop Avenue", "Angel Circle", "Alpine Circle", "Bannatyne Street", "Brunswick Crescent", "Atlas Circle", "Medway Corner", "Gresham Highway", "Northwood Park", "Alpine Ride", "Pound Woods", "Elmwood Isaf", "Causeway Farm", "Roman Gardens" };

            switch (attribute)
            {
                case "Streets":
                    attr = streets;
                    break;
                case "Cities":
                    attr = cities;
                    break;
                case "States":
                    attr = states;
                    break;
                case "Neighbourhoods":
                    attr = neighbourhoods;
                    break;
                case "Names":
                    attr = names;
                    break;
                case "Products":
                    attr = products;
                    break;
                default:
                    attr = null;
                    break;
            }

            var rand = new Random();
            if (idx == -1)
                return attr[rand.Next(0, attr.Length - 1)];
            else
                return attr[idx];
        }

        public static string GenerateLoremIpsum(int minWords, int maxWords,
        int minSentences, int maxSentences)
        {
            var words = new[]{"a", "ac", "accumsan", "ad", "adipiscing", "aenean", "aliquam", "aliquet",
            "amet", "ante", "aptent", "arcu", "at", "auctor", "augue", "bibendum", "blandit", "class",
            "commodo", "condimentum", "congue", "consectetuer", "consequat", "conubia", "convallis",
            "cras", "cubilia", "cum", "curabitur", "curae;", "cursus", "dapibus", "diam", "dictum",
            "dignissim", "dis", "dolor", "donec", "dui", "duis", "egestas", "eget", "eleifend",
            "elementum", "elit", "enim", "erat", "eros", "est", "et", "etiam", "eu", "euismod",
            "facilisi", "facilisis", "fames", "faucibus", "felis", "fermentum", "feugiat", "fringilla",
            "fusce", "gravida", "habitant", "hendrerit", "hymenaeos", "iaculis", "id", "imperdiet", "in",
            "inceptos", "integer", "interdum", "ipsum", "justo", "lacinia", "lacus", "laoreet", "lectus",
            "leo", "libero", "ligula", "litora", "lobortis", "lorem", "luctus", "maecenas", "magna",
            "magnis", "malesuada", "massa", "mattis", "mauris", "metus", "mi", "molestie", "mollis",
            "montes", "morbi", "mus", "nam", "nascetur", "natoque", "nec", "neque", "netus", "nibh",
            "nisi", "nisl", "non", "nonummy", "nostra", "nulla", "nullam", "nunc", "odio", "orci",
            "ornare", "parturient", "pede", "pellentesque", "penatibus", "per", "pharetra", "phasellus",
            "placerat", "porta", "porttitor", "posuere", "praesent", "pretium", "primis", "proin",
            "pulvinar", "purus", "quam", "quis", "quisque", "rhoncus", "ridiculus", "risus", "rutrum",
            "sagittis", "sapien", "scelerisque", "sed", "sem", "semper", "senectus", "sit", "sociis",
            "sociosqu", "sodales", "sollicitudin", "suscipit", "suspendisse", "taciti", "tellus",
            "tempor", "tempus", "tincidunt", "torquent", "tortor", "tristique", "turpis", "ullamcorper",
            "ultrices", "ultricies", "urna", "ut", "varius", "vehicula", "vel", "velit", "venenatis",
            "vestibulum", "vitae", "vivamus", "viverra", "volutpat", "vulputate"
            };

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();
            for (int s = 0; s < numSentences; s++)
            {
                for (int w = 0; w < numWords; w++)
                {
                    if (w > 0) { result.Append(" "); }
                    result.Append(words[rand.Next(words.Length)]);
                }
                result.Append(". ");
            }
            return result.ToString();
        }
    }
}