//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using KSP;
//using UnityEngine;
//using KSPPluginFramework;

//namespace KSPTips
//{

//    public class TipBuilding
//    {

//        private SpaceCenterBuilding _building;

//        public SpaceCenterBuilding building
//        {
//            get { return _building; }
//            set { 
//                _building = value; 
//                //extract the level stuff
//                if (this.FacilityExists)
//                ExtractLevelInfo(this.Facility);
//            }
//        }

//        private void ExtractLevelInfo(Upgradeables.UpgradeableFacility facility)
//        {
//            FacilityLevels = new List<TipFacilityLevel>();
//            for (int i = 0; i <= facility.MaxLevel; i++)
//            {
//                TipFacilityLevel newlevel = new TipFacilityLevel();
//                newlevel.Level = i + 1;
//                newlevel.Text = facility.GetLevelText(i);
//                FacilityLevels.Add(newlevel);
//            }
//        }

//        public List<TipFacilityLevel> FacilityLevels;

//        public String Name { get {return building.buildingInfoName;}}
//        public String Description { get{return building.buildingDescription;} }
//        public String FacilityName { get{return building.facilityName;}}

//        public Boolean FacilityExists { get { return building.Facility != null; } }
//        public Upgradeables.UpgradeableFacility Facility { get { return building.Facility; } }

//    }

//    public class TipFacilityLevel{
//        public int Level { get; set; }
//        public String Text { get; set; }
//    }

//    public class KSCMouseOver : MonoBehaviourExtended
//    {
//        public Dictionary<String, TipBuilding> lstBuildings;

//        public static String LastBuilding = "";

//        internal override void Start()
//        {
//            lstBuildings = new Dictionary<string, TipBuilding>();
//            //Getting Building List
//            FindObjectsOfType<SpaceCenterBuilding>()
//               .ToList()
//               .ForEach(b => {
//                   TipBuilding newBuilding = new TipBuilding();
//                   newBuilding.building = b;
//                   lstBuildings.Add(b.buildingInfoName, newBuilding);
//               });

//            foreach (TipBuilding item in lstBuildings.Values)
//            {
//                print(String.Format("{0}-{1}-{2}-{3}",item.Name,item.Description,item.FacilityName,item.FacilityExists));
//            }
//        }

//        internal override void Update()
//        {
//            String CurrentBuilding = GetComponent<ScreenSafeGUIText>().text;
//            if (CurrentBuilding != LastBuilding)
//            {
//                if (LastBuilding != "")
//                    onMouseExit(lstBuildings[LastBuilding]);

//                if (CurrentBuilding != "")
//                    onMouseEnter(lstBuildings[CurrentBuilding]);
//                LastBuilding = CurrentBuilding;
//            }
//        }

//        public delegate void MouseOverEvent(TipBuilding building);

//        public static event MouseOverEvent onMouseEnter;
//        public static event MouseOverEvent onMouseExit;

//        public static void MouseOverMonitor(TipBuilding building)
//        {
//            if (onMouseEnter != null)
//                onMouseEnter(building);
//        }
//        public static void MouseExitMonitor(TipBuilding building)
//        {
//            if (onMouseExit != null)
//                onMouseExit(building);
//        }
//    }
//}
