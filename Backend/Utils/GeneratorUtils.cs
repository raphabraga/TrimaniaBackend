using System;
using System.Collections.Generic;
using System.IO;
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

            var products = new[] { "Military Noise Refiner Engine", "Surface Soundwave Obscurator Widget", "Explosive Casualty Magnetizer Doohickey", "Super Fire Reproducer Apparatus", "Combat Monitron Interface", "Laser Enhancomatic Engine", "Portable Fire Possessor Matrix", "Atmospheric Bone Conveyor Engine", "Mimic Evaluator Tool", "Load Obscurator MachineKit", "Electromagnetic Light Automatron Device", "Comprehensive Sea Generator Apparatus", "Exam Maximizer Kit", "Automated Plague Mechanizer Thingymajig", "Mini Barrier Director Gizmo", "Automated Sea Possessor Network", "Medicine Controller MachineKit", "Precise Survival Detectron Matrix", "Superior Toxin Attractomatic Machine", "Solar Rescue Telelocator", "Altered Combat Transferator Engine", "Modified Virus Encrypter Device", "Medium Battle Concealotron Matrix", "Optimized Aid Decoder Device", "Improved Obstacle Catalyzer Widget", "Warfare Concealotron Kit", "Military Shock Separatron Network", "Atmospheric Antidote Mechanizer Interface", "Titanium Aid Automatron Apparatus", "Aquatic Pollution Possessor Device", "Extraordinary Antivenom Mutator Apparatus", "Robotic Transport Enticer MachineKit", "Computerized Termination Transfigurator MachineKit", "Precise Ice Manipulator Device", "Explosive Service Enticer", "Precise Flood Processor", "Systematic Soundwave Analyzer Gizmo", "Titanium Tree Transmitter Device", "Synthetic Cure Distrubutor Engine", "Experimatic Sample Mutator Network", "Gizmatic Pollution Disconnector MachineKit", "Tree Analyzer Apparatus", "Super Data Enticer Tool", "Medium Construction Telelocator Mechanism", "Extrematic Virus Controller Doohickey", "Selfaware First Aid Analytron Matrix", "Crime Mechanizer Network", "Storage Enhancomatic Tool", "Analytical Comfort Mutator Engine", "Aura Transmitter Matrix", "Extended Aura Transferator Kit", "Mini Radiation Morphomatic", "Laser Intensitron Doohickey", "Selfaware Broadcast Transporter Engine", "Super Survival Pullomatic Interface", "Hazard Scrambler Matrix", "Service Detector Tool", "Selfaware Biopsy Teleporter Tool", "Propulsion Minimizer Network", "Reinforced Guidance Diagnoser Widget", "Mobile Life Modulator Network", "Spirit Disseminator Widget", "Auxiliary Disaster Decoder Network", "Altered Service Mutator Widget", "Code Detector Thingymajig", "Optimized Surgeon Monitron Engine", "Wound Transmogrifier Tool", "Modified Motion Reproducer Thingymajig", "Computerized Pollution Minimizer", "Solar Rescue Pullomatic Interface", "Reinforced Luggage Diagnoser Device", "Guidance Minimizer Tool", "Experitron Hologram Attractomatic Matrix", "Ammo Generator Gizmo", "Electronic Remedy Transmogrifier Interface", "Systematic Assist Conveyor", "Noise Inducer Apparatus", "Reformed Strategy Analytron Widget", "Sample Secretor Apparatus", "Precise First Aid Transmogrifier Thingymajig", "Weather Refiner Network", "Optimal Antidote Jumbler Engine", "Road Revealotron Apparatus", "Solar Echo Inducer Matrix", "Medical Code Disconnector Interface", "Global Cooking Concealotron", "Portable Assist Analyzer Interface", "Surface Light Revealotron Tool", "Medical Air Evaluator Machine", "Medical Crime Transformer MachineKit", "Portable Soil Circulator Doodad", "Service Magnetizer Tool", "Explosive First Aid Possessor MachineKit", "Experimatic Clay Detectron Mechanism", "Electronic Survival Regenerator Engine", "Reformed Load Processor Kit", "First Aid Transmogrifier Thingymajig", "Optimized Warfare Pullomatic Apparatus", "Modified Mimic Jumbler Engine", "Global Cooking Monitron Machine" };

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