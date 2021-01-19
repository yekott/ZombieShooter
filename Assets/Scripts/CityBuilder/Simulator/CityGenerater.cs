using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
namespace Zombie
{
    public enum MapSize { Map_Small, Map_Medium, Map_Huge }
    public enum CellType { Road, SideWalk, Intersection, NormalField, Corner}
    public enum CellLocation { None, UpLeft, UpRight, DownLeft, DownRight}
    public enum IsLandScape { LandScape, Vertical}
    public enum IntersectionDirection { RightUp, RightDown, LeftUp, LeftDown}
    public class CityGenerater : MonoBehaviour
    {
        public MapSize mapSize;
        int CityWidht;
        int CityHeight;
        int CitySquare;
        int CITYSQUARE;

        private static int pathWidht = 5;

        public CityCell[] cityCell;

        public GameObject roadPlain;
        public Plains sideWalkPlains;
        public GameObject interSectionPlain;
        public Plains paveMentPains;
        public Plains cornerPlains;

        public BuildingList[] buildingList;
        public Material CustomMaterial;
        NavMeshSurface meshSurface;

        public List<RoadSegment> RoadSegments { get; private set; }
        public List<Intersection> Intersections { get; private set; }
        RoadNetWork network;

        public GameObject[] OutSideFloor;

        public Camera miniMapCamera;
        public RawImage backimage;
        public RawImage units;

        int spotX;
        int spotZ;

        public int GetSpotX()
        {
            return spotX;
        }
        public int GetSpotZ()
        {
            return spotZ;
        }

        public void Execute()
        {
            MapGenerate();
            FieldGenerater();
            StartCoroutine(MiniMapCamera());
        }

        IEnumerator MiniMapCamera()
        {
            yield return new WaitForEndOfFrame();
            miniMapCamera = GetComponentInChildren<Camera>();

            float mapLocation = CityWidht * 5/2;
            miniMapCamera.cullingMask = (1 << 9 | 1 << 10|1<<31);
            miniMapCamera.transform.position = new Vector3(mapLocation-5, mapLocation, mapLocation-5);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = miniMapCamera.targetTexture;
            miniMapCamera.Render();

            Texture2D image = new Texture2D(miniMapCamera.targetTexture.width, miniMapCamera.targetTexture.height, TextureFormat.ARGB32, false);
            image.ReadPixels(new Rect(0, 0, miniMapCamera.targetTexture.width, miniMapCamera.targetTexture.height), 0, 0);
            image.Apply();

            backimage.texture = image;


            miniMapCamera.cullingMask = (1 << 11 | 1 << 12);
        }

        public void MapGenerate()
        {
            if (mapSize == MapSize.Map_Small)
            {
                CityWidht = CityHeight = 33;
            }
            else if (mapSize == MapSize.Map_Medium)
            {
                CityWidht = CityHeight = 65;
            }
            else
            {
                CityWidht = CityHeight = 97;
            }
            CitySquare = CityWidht - 1;
            CITYSQUARE = CitySquare * CitySquare;
            int length = CitySquare * CitySquare;
            cityCell = new CityCell[length];
            for (int i = 0; i < length; i++)
            {
                cityCell[i] = new CityCell();
            }
            Intersections = new List<Intersection>();

            int CityHalf = CitySquare / 2;

            this.network = new RoadNetWork(CitySquare, CitySquare);
            spotX = Random.Range(CityHalf - 9, CityHalf + 9);
            spotZ = Random.Range(CityHalf - 9, CityHalf + 9);
            this.network.InitRoadCentre(spotX, spotZ, CityWidht, CityHeight);

            this.network.SplitSegments(0);
            this.network.SplitSegments(0);
            this.network.SplitSegments(1);
            this.network.SplitSegments(1);
            this.network.SplitSegments(2);
            this.network.SplitSegments(3);
        }

        public void FieldGenerater()
        {
            //세그먼트를 citycell 배열에 넣음
            foreach (RoadSegment segment in this.network.roadSegments)
            {
                int OriginX = segment.PointA.point[0];
                int OriginZ = segment.PointA.point[1];

                int TargetX = segment.PointB.point[0];
                int TargetZ = segment.PointB.point[1];

                int i; // 더 낮은 수
                int t; // 더 높은 수
                int staticValue;


                if (OriginX == TargetX) // X 위치가 같다면? 수평
                {
                    staticValue = TargetX;
                    if (OriginZ > TargetZ)
                    {
                        i = TargetZ;
                        t = OriginZ;
                    }
                    else
                    {
                        i = OriginZ;
                        t = TargetZ;
                    }
                    for (; i < t ; i++)
                    {

                        if (staticValue > 0)
                        {
                            int CellLocation = i * CitySquare + (staticValue - 1);
                            if (CellLocation >= CITYSQUARE) continue;
                            cityCell[CellLocation].cellType = CellType.Road;
                            cityCell[CellLocation].isOccupied = true;
                            cityCell[CellLocation].isVertical = true;
                            if (staticValue > 1)
                            {
                                int SDLocation = i * CitySquare + (staticValue - 2);
                                if (SDLocation >= CITYSQUARE) continue;
                                if (!cityCell[SDLocation].isOccupied)
                                {
                                    cityCell[SDLocation].cellType = CellType.SideWalk;
                                    cityCell[SDLocation].isLeft = true;
                                    cityCell[SDLocation].cornerCount++;
                                }
                            }
                        }

                        if (staticValue < CityWidht)
                        {
                            int CellLocation = i * CitySquare + staticValue;
                            if (CellLocation >= CITYSQUARE) continue;
                            cityCell[CellLocation].cellType = CellType.Road;
                            cityCell[CellLocation].isOccupied = true;
                            cityCell[CellLocation].isVertical = true;
                            if (staticValue < CitySquare)
                            {
                                int SDLocation = i * CitySquare + (staticValue + 1);
                                if (SDLocation >= CITYSQUARE) continue;
                                if (!cityCell[SDLocation].isOccupied)
                                {
                                    cityCell[SDLocation].cellType = CellType.SideWalk;
                                    cityCell[SDLocation].cornerCount++;
                                }
                            }
                        }

                    }
                }
                else //z 축이 동일함
                {
                    staticValue = TargetZ;
                    if (OriginX > TargetX)
                    {
                        i = TargetX;
                        t = OriginX;
                    }
                    else
                    {
                        i = OriginX;
                        t = TargetX;
                    }

                    for (; i < t; i++)
                    {
                        if (i >= CitySquare) continue;
                        if (staticValue > 0)
                        {
                            int CellLocation = (staticValue - 1) * CitySquare + i;
                            if (CellLocation >= CITYSQUARE) continue;
                            cityCell[CellLocation].cellType = CellType.Road;
                            cityCell[CellLocation].isVertical = false;
                            cityCell[CellLocation].isOccupied = true;
                            if (staticValue > 1)
                            {
                                int SDLocation = (staticValue - 2) * CitySquare + i;
                                if (SDLocation >= CITYSQUARE) continue;
                                if (!cityCell[SDLocation].isOccupied)
                                {
                                    cityCell[SDLocation].cellType = CellType.SideWalk;
                                    cityCell[SDLocation].isVertical = false;
                                    cityCell[SDLocation].cornerCount++;
                                }
                            }
                        }

                        if (staticValue < CityWidht)
                        {
                            int CellLocation = staticValue * CitySquare + i;
                            
                            if (CellLocation >= CITYSQUARE) continue;
                            cityCell[CellLocation].cellType = CellType.Road;
                            cityCell[CellLocation].isVertical = false;
                            cityCell[CellLocation].isOccupied = true;
                            if (staticValue < CitySquare)
                            {
                                int SDLocation = (staticValue + 1) * CitySquare + i;
                                if (SDLocation >= CITYSQUARE) continue;
                                if (!cityCell[SDLocation].isOccupied)
                                {
                                    cityCell[SDLocation].cellType = CellType.SideWalk;
                                    cityCell[SDLocation].isVertical = false;
                                    cityCell[SDLocation].isUp = true;
                                    cityCell[SDLocation].cornerCount++;
                                }
                            }
                        }

                    }
                }
            }
            
            //교차지점을 citycell 에 확정시키고 교차로처럼 만듬
            foreach (Intersection inter in this.network.intersections)
            {
                if (this.Intersections.Exists(p => p.IsThisOne(inter)))
                    continue;
                int x = inter.points[0].point[0];
                int z = inter.points[0].point[1];

                int xLeft = x - 1;
                int zDown = z - 1;

                int leftup = xLeft + z * CitySquare;
                int leftdown = xLeft + zDown * CitySquare;
                int rightup = x + z * CitySquare;
                int rightdown = x + zDown * CitySquare;
                cityCell[x + z * CitySquare].cellType = CellType.Intersection;
                if(xLeft < 0) // left 2 intersecter must be ignore
                {
                    if (zDown < 0)
                    {
                        cityCell[rightup].cellType = CellType.Intersection;
                        cityCell[rightup].isOccupied = true;
                    }
                    else if(z == CitySquare)
                    {
                        cityCell[rightdown].cellType = CellType.Intersection;
                        cityCell[rightdown].isOccupied = true;
                    }
                    else
                    {
                        cityCell[rightup].cellType = CellType.Intersection;
                        cityCell[rightdown].cellType = CellType.Intersection;
                        cityCell[rightup].isOccupied = true;
                        cityCell[rightdown].isOccupied = true;
                    }
                }
                else if(x == CitySquare)// right 2 intersecter must be ignore
                {
                    if (zDown < 0)
                    {
                        cityCell[leftup].cellType = CellType.Intersection;
                        cityCell[leftup].isOccupied = true;
                    }
                    else if (z == CitySquare)
                    {
                        cityCell[leftdown].cellType = CellType.Intersection;
                        cityCell[leftdown].isOccupied = true;
                    }
                    else
                    {
                        cityCell[leftup].cellType = CellType.Intersection;
                        cityCell[leftdown].cellType = CellType.Intersection;
                        cityCell[leftup].isOccupied = true;
                        cityCell[leftdown].isOccupied = true;
                    }

                }
                else
                {
                    if (zDown < 0)
                    {
                        cityCell[leftup].cellType = CellType.Intersection;
                        cityCell[rightup].cellType = CellType.Intersection;
                        cityCell[leftup].isOccupied = true;
                        cityCell[rightup].isOccupied = true;
                    }
                    else if (z == CitySquare)
                    {
                        cityCell[leftdown].cellType = CellType.Intersection;
                        cityCell[rightdown].cellType = CellType.Intersection;
                        cityCell[leftdown].isOccupied = true;
                        cityCell[rightdown].isOccupied = true;
                    }
                    else
                    {
                        cityCell[leftup].cellType = CellType.Intersection;
                        cityCell[rightup].cellType = CellType.Intersection;
                        cityCell[leftdown].cellType = CellType.Intersection;
                        cityCell[rightdown].cellType = CellType.Intersection;

                        cityCell[leftup].isOccupied = true;
                        cityCell[rightup].isOccupied = true;
                        cityCell[leftdown].isOccupied = true;
                        cityCell[rightdown].isOccupied = true;
                    }
                }


                int xminus2 = x - 2;
                int zminus2 = z - 2;
                int xplus1 = x + 1;
                int zplus1 = z + 1;

                int leftup2 = xminus2 + zplus1 * CitySquare;
                int leftdown2 = xminus2 + zminus2 * CitySquare;
                int rightup2 = xplus1 + zplus1 * CitySquare;
                int rightdown2 = xplus1 + zminus2 * CitySquare;

                if(xminus2 < 0)
                {
                    if (zminus2 >= 0)
                    {
                        if (cityCell[rightup2].cornerCount >= 2)
                        {
                            cityCell[rightup2].cellType = CellType.Corner;
                            cityCell[rightup2].IDerectionType = IntersectionDirection.RightUp;
                        }
                    }

                    if(zplus1 < CitySquare)
                    {
                        if (cityCell[rightdown2].cornerCount >= 2)
                        {
                            cityCell[rightdown2].cellType = CellType.Corner;
                            cityCell[rightdown2].IDerectionType = IntersectionDirection.RightDown;
                        }
                    }
                }
                else if (xplus1 >= CitySquare)
                {
                    if(zminus2 >= 0)
                    {
                        if (cityCell[leftup2].cornerCount >= 2)
                        {
                            cityCell[leftup2].cellType = CellType.Corner;
                            cityCell[leftup2].IDerectionType = IntersectionDirection.LeftUp;
                        }
                    }

                    if(zplus1 < CitySquare)
                    {
                        if (cityCell[leftdown2].cornerCount >= 2)
                        {
                            cityCell[leftdown2].cellType = CellType.Corner;
                            cityCell[leftdown2].IDerectionType = IntersectionDirection.LeftDown;
                        }
                    }
                }
                else
                {
                    if (zminus2 >= 0)
                    {
                        if (cityCell[leftup2].cornerCount >= 2)
                        {
                            cityCell[leftup2].cellType = CellType.Corner;
                            cityCell[leftup2].IDerectionType = IntersectionDirection.LeftUp;
                        }

                        if (cityCell[rightup2].cornerCount >= 2)
                        {
                            cityCell[rightup2].cellType = CellType.Corner;
                            cityCell[rightup2].IDerectionType = IntersectionDirection.RightUp;
                        }
                    }

                    if (zplus1 < CitySquare)
                    {
                        if (cityCell[leftdown2].cornerCount >= 2)
                        {
                            cityCell[leftdown2].cellType = CellType.Corner;
                            cityCell[leftdown2].IDerectionType = IntersectionDirection.LeftDown;
                        }
                        if (cityCell[rightdown2].cornerCount >= 2)
                        {
                            cityCell[rightdown2].cellType = CellType.Corner;
                            cityCell[rightdown2].IDerectionType = IntersectionDirection.RightDown;
                        }
                    }
                }
            }

            for(int i = 0; i< CitySquare; i++)
            {
                for(int j = 0; j < CitySquare; j++)
                {
                    int position = j + i * CitySquare;
                    GameObject go;


                    // 셀 타입별로 정리
                    if (cityCell[position].cellType == CellType.SideWalk)
                    {
                        if (Random.Range(0, 100) > 5)
                        {
                            go = Instantiate(sideWalkPlains.Fields[Random.Range(0, 2)]);
                        }
                        else
                        {
                            go = Instantiate(sideWalkPlains.Fields[Random.Range(2, sideWalkPlains.Fields.Length)]);
                        }

                        if (!cityCell[position].isVertical)
                        {
                            if (cityCell[position].isUp)
                            {
                                go.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                            }
                            else
                            {
                                go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            }
                        }
                        else
                        {
                            if (cityCell[position].cellType == CellType.SideWalk)
                            {
                                if (cityCell[position].isLeft)
                                {
                                    go.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                }
                                else
                                {
                                    go.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                }
                            }

                        }
                    }
                    else if (cityCell[position].cellType == CellType.Road)
                    {
                        go = Instantiate(roadPlain);
                        if (!cityCell[position].isVertical)
                        {
                            go.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        }
                    }
                    else if (cityCell[position].cellType == CellType.Intersection)
                    {
                        go = Instantiate(interSectionPlain);
                    }
                    else if (cityCell[position].cellType == CellType.Corner)
                    {
                        go = Instantiate(cornerPlains.Fields[Random.Range(0, cornerPlains.Fields.Length)]);
                        if (cityCell[position].IDerectionType == IntersectionDirection.LeftDown)
                        {
                            //go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                        }
                        else if (cityCell[position].IDerectionType == IntersectionDirection.RightDown)
                        {
                            go.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

                        }
                        else if (cityCell[position].IDerectionType == IntersectionDirection.RightUp)
                        {
                            go.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                        }
                        else
                        {
                            go.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        }
                    }
                    else
                    {
                        if (Random.Range(0, 100) > 5)
                        {
                            go = Instantiate(paveMentPains.Fields[Random.Range(0, 2)]);
                        }
                        else
                        {
                            go = Instantiate(paveMentPains.Fields[Random.Range(2, paveMentPains.Fields.Length)]);
                        }
                    }
                    go.transform.position = new Vector3(j * 5, 0, i * 5);
                    //불러온 floor를 레이어랑 부모관계 생성
                    go.transform.parent = this.gameObject.transform;
                    go.layer = LayerMask.NameToLayer("Floor");
                }
            }

            BuildingLocationAndBuild(0);

            //네비게이션 세팅
            UpdateNavMeshSurface();
            OutSideFloorBuild();
        }

        //바깥쪽 플로어 세팅
        public void OutSideFloorBuild()
        {
            int localMul = CitySquare / 32;
            for (int i = 0; i < localMul; i++)
            {
                //아래
                GameObject OutSideField = Instantiate(OutSideFloor[0]);
                OutSideField.transform.position = new Vector3(80 + i * 160 - 2.5f, 0, -32.5f);

                //왼쪽
                GameObject OutSideField2 = Instantiate(OutSideFloor[0]);
                OutSideField2.transform.position = new Vector3(-32.5f, 0, 80 + i * 160 - 2.5f);
                OutSideField2.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));


                GameObject OutSideField3 = Instantiate(OutSideFloor[0]);
                OutSideField3.transform.position = new Vector3(80 + i * 160 - 2.5f, 0, 27.5f + CitySquare * 5);

                GameObject OutSideField4 = Instantiate(OutSideFloor[0]);
                OutSideField4.transform.position = new Vector3(CitySquare * 5 + 27.5f, 0, 80 + i * 160 - 2.5f);
                OutSideField4.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            GameObject OutSideConer1 = Instantiate(OutSideFloor[1]);
            OutSideConer1.transform.position = new Vector3(-32.5f, 0, -32.5f);
            GameObject OutSideConer2 = Instantiate(OutSideFloor[1]);
            OutSideConer2.transform.position = new Vector3(-32.5f, 0, CitySquare * 5 + 27.5f);
            GameObject OutSideConer3 = Instantiate(OutSideFloor[1]);
            OutSideConer3.transform.position = new Vector3(CitySquare * 5 + 27.5f, 0, -32.5f);
            GameObject OutSideConer4 = Instantiate(OutSideFloor[1]);
            OutSideConer4.transform.position = new Vector3(CitySquare * 5 + 27.5f, 0, CitySquare * 5 + 27.5f);


        }

        //navmeshSurface 동적생성
        public void UpdateNavMeshSurface() 
        {
            this.gameObject.AddComponent<NavMeshSurface>();
            meshSurface = GetComponent<NavMeshSurface>();
            meshSurface.BuildNavMesh();
        }

        //true = citycell 배열을 넘어섰거나, citycell 내부 배열 찾던중 이미 무언가가 그 위치에 존재할때
        //false = 찾는 위치에 아무것도 없을때
        public bool IsOccupation(int x, int z, int width, int height) 
        {
            int XLimit = x + width;
            int ZLimit = z + height;
            for (int i = z; i<ZLimit; i++)
            {
                for(int j = x; j<XLimit; j++)
                {
                    if (i * CitySquare + j >= CITYSQUARE) return true;
                    if (cityCell[i * CitySquare + j].isOccupied)
                        return true;
                }
            }
            return false;
        }


        public void BuildingLocationAndBuild(int ListNum)
        {
            
            //빌딩 배치
            for (int i = 0; i < CitySquare; i++)
            {
                for (int j = 0; j < CitySquare; j++)
                {
                    int BuildingNum = Random.Range(0,buildingList[ListNum].buildings.Length);
                    float offSetX = buildingList[ListNum].buildings[BuildingNum].offSetX;
                    float offSetZ = buildingList[ListNum].buildings[BuildingNum].offSetZ;
                    int WidthX = buildingList[ListNum].buildings[BuildingNum].WidthX;
                    int HeightZ = buildingList[ListNum].buildings[BuildingNum].HeightZ;
                    bool CantBuildLand = IsOccupation(j, i, WidthX, HeightZ);
                    bool CantBuildVert = IsOccupation(j, i, HeightZ, WidthX);

                    bool LandScape = false;
                    bool Vertical = false;


                    if (CantBuildLand && CantBuildVert)
                    {
                        continue;
                    }
                    else
                    {
                        if (!CantBuildLand)
                            LandScape = true; //가로로 짓기 가능
                        if (!CantBuildVert)
                            Vertical = true; // 세로로 짓기 가능
                    }
                    if (LandScape)
                    {
                        if (j + WidthX > CitySquare)
                            continue;
                        if (i + HeightZ > CitySquare)
                            continue;
                        BuildBuilding(j, i, WidthX, HeightZ, offSetX, offSetZ, IsLandScape.LandScape,  ListNum,  BuildingNum);
                    }
                    else if (Vertical)
                    {
                        if (j + buildingList[ListNum].buildings[BuildingNum].HeightZ > CitySquare)
                            continue;
                        if (i + WidthX > CitySquare)
                            continue;
                        BuildBuilding(j, i, WidthX, HeightZ, offSetZ, offSetX, IsLandScape.Vertical,  ListNum,  BuildingNum);
                    }

                }
            }

        }
        // 빌딩 짓는 함수인데 offset들은 건물들이 정중앙에 위치해 있을 수 있게 해주는 놈들임.
        public void BuildBuilding(int x, int z, int width, int height , float offsetX, float offsetZ, IsLandScape isLandScape, int RandomListNum, int RandomBuildNum) 
        {
            int XLimit;
            int ZLimit;
            if (isLandScape == IsLandScape.Vertical)
            {
                XLimit = x + height;
                ZLimit = z + width;
            }
            else
            {
                XLimit = x + width;
                ZLimit = z + height;
            }
            for (int i = z; i < ZLimit; i++)
            {
                for (int j = x; j < XLimit; j++)
                {
                    cityCell[i * CitySquare + j].isOccupied = true;
                }
            }
            GameObject Building = Instantiate(buildingList[RandomListNum].buildings[RandomBuildNum].Prefab);
            Building.transform.position = new Vector3((x + x + width - 1) * 2.5f + offsetX, 0.01f, (z + z + height - 1) * 2.5f + offsetZ);
            if (isLandScape != IsLandScape.LandScape)
            {
                Building.transform.position = new Vector3((x + x + height - 1) * 2.5f + offsetZ, 0.01f, (z + z + width - 1) * 2.5f - offsetX);
                Building.transform.rotation = Quaternion.Euler( new Vector3(0, 90, 0));
            }
            Building.transform.parent = this.gameObject.transform;
            MeshRenderer meshRenderer = Building.GetComponent<MeshRenderer>();
            meshRenderer.material = CustomMaterial;
            Building.layer = LayerMask.NameToLayer("Building");

        }
    }
    public class CityCell
    {
        public bool isOccupied = false; //현재 위치에 도로, 건물이 있나요?

        //도로 관련
        public bool isVertical = true;  //현재 도로가 수평이어야 하나요 수직이어야 하나요?
        public bool isLeft = false;     //도로가 x 양뱡향 기준으로 왼쪽에 있나요?
        public bool isUp = false;       //도로가 z 양방향 기준으로 위쪽에 있나요?
        public CellLocation cellLocation; // 사이드워크 코너부분이 어디에 있나요?
        public CellType cellType;       // 도로인지, 사이드 워크인지
        public IntersectionDirection IDerectionType;
        public int cornerCount = 0;

        public CityCell()
        {
            cellType = CellType.NormalField;
            cellLocation = CellLocation.None;
        }
    }

    public class RoadNetWork
    {
        public List<RoadSegment> roadSegments;
        public List<Intersection> intersections;

        int LimitWidth;
        float shortCutOff = 5.5f;
        float closeCutOff = 5.5f;
        public float scale { get; private set; }

        public RoadNetWork(int Width, float scale)
        {
            this.LimitWidth = Width;
            this.roadSegments = new List<RoadSegment>();
            this.intersections = new List<Intersection>();
            this.scale = scale;
        }

        public void SplitSegments(int level = 0)
        {
            List<RoadSegment> segments = new List<RoadSegment>(this.roadSegments);

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].Level == level)
                {
                    this.SplitSegment(segments[i]);
                }
            }
        }

        private void SplitSegment(RoadSegment segment)
        {
            float splitDistance = Random.Range(0.33f, 0.66f);

            //get split distance
            Vector3 p1 = new Vector3(segment.PointA.point[0], 0, segment.PointA.point[1]);
            Vector3 p2 = new Vector3(segment.PointB.point[0], 0, segment.PointB.point[1]);
            float length = Vector3.Distance(p1, p2);
            length *= splitDistance;
            //get direction vector for segment
            Vector3 direction = (p1 - p2).normalized;

            //get new point and patch the segment
            Vector3 newPoint = p2 + (direction * length);

            //calaculate other new point
            Vector3 per;
            if(Mathf.Abs(direction.x) == 1)
            {
                per = new Vector3(0, 0, 1);
            }
            else
            {
                per = new Vector3(1, 0, 0);
            }

            float newLength = this.scale / ((segment.Level + 1) * Random.Range(1f, 2f)) ;

            Vector3 newPointEnd = newPoint + (per * newLength);

            if(newPointEnd.x >= LimitWidth)
            {
                newPointEnd.x = LimitWidth;
            }else if(newPointEnd.x < 0)
            {
                newPointEnd.x = 0;
            }else if (newPointEnd.z >= LimitWidth)
            {
                newPointEnd.z = LimitWidth;
            }
            else if (newPointEnd.z < 0)
            {
                newPointEnd.z = 0;
            }

            RoadSegment newSegment = new RoadSegment(new RoadPoint(new int[] { (int)newPoint.x, (int)newPoint.z }), new RoadPoint(new int[] { (int)newPointEnd.x, (int)newPointEnd.z }), segment.Level + 1);

            Vector3 perA = per * -1;

            Vector3 newPointEndOther = newPoint + (perA * newLength);


            if (newPointEndOther.x >= LimitWidth)
            {
                newPointEndOther.x = LimitWidth;
            }
            else if (newPointEndOther.x < 0)
            {
                newPointEndOther.x = 0;
            }
            else if (newPointEndOther.z >= LimitWidth)
            {
                newPointEndOther.z = LimitWidth;
            }
            else if (newPointEndOther.z < 0)
            {
                newPointEndOther.z = 0;
            }
            RoadSegment newSegmentOther = new RoadSegment(new RoadPoint(new int[] { (int)newPoint.x, (int)newPoint.z }), new RoadPoint(new int[] { (int)newPointEndOther.x, (int)newPointEndOther.z }), segment.Level + 1);

            //check what segments to add and add them
            bool seg1 = false;
            bool seg2 = false;

            bool with1 = this.SegmentWithin(newSegment, closeCutOff);
            bool with2 = this.SegmentWithin(newSegmentOther, closeCutOff);


            if (!with1)
            {
                Vector2 intersection = Vector3.zero;
                RoadSegment other = null;

                int iCount = segmentIntersection(newSegment, out intersection, out other, segment);

                if (iCount <= 1)
                {
                    this.roadSegments.RemoveAll(p => p.IsEqual(newSegment));
                    this.roadSegments.Add(newSegment);
                    seg1 = true;
                }

                if (iCount == 1)
                {
                    RoadSegment[] segmentsA = this.patchSegment(other, new RoadPoint((int)intersection[0], (int)intersection[1], other));
                    RoadSegment[] segmentsB = this.patchSegment(newSegment, new RoadPoint((int)intersection[0], (int)intersection[1], newSegment));

                    //kill very short dead-ends
                    bool sa = segmentsA[0].SegmentLength() > shortCutOff;
                    bool sb = segmentsA[1].SegmentLength() > shortCutOff;
                    bool sc = segmentsB[0].SegmentLength() > shortCutOff;
                    bool sd = segmentsB[1].SegmentLength() > shortCutOff;


                    List<RoadPoint> points = new List<RoadPoint>();
                    if (sa)
                        points.Add(segmentsA[0].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsA[0]));

                    if (sb)
                        points.Add(segmentsA[1].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsA[1]));

                    if (sc)
                        points.Add(segmentsB[0].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsB[0]));

                    if (sd)
                        points.Add(segmentsB[1].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsB[1]));

                    Intersection inter = new Intersection(points);
                    this.intersections.Add(inter);
                }
            }

            //other side of intersection
            if (!with2)
            {
                Vector2 intersection = Vector3.zero;
                RoadSegment other = null;

                int iCount = segmentIntersection(newSegmentOther, out intersection, out other, segment);

                if (iCount <= 1)
                {
                    this.roadSegments.RemoveAll(p => p.IsEqual(newSegmentOther));
                    this.roadSegments.Add(newSegmentOther);
                    seg2 = true;
                }

                if (iCount == 1)
                {
                    RoadSegment[] segmentsA = this.patchSegment(other, new RoadPoint((int)intersection[0], (int)intersection[1], other));
                    RoadSegment[] segmentsB = this.patchSegment(newSegmentOther, new RoadPoint((int)intersection[0], (int)intersection[1], newSegmentOther));

                    //kill very short dead-ends
                    bool sa = segmentsA[0].SegmentLength() > shortCutOff;
                    bool sb = segmentsA[1].SegmentLength() > shortCutOff;
                    bool sc = segmentsB[0].SegmentLength() > shortCutOff;
                    bool sd = segmentsB[1].SegmentLength() > shortCutOff;

                    List<RoadPoint> points = new List<RoadPoint>();
                    if (sa)
                        points.Add(segmentsA[0].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsA[0]));

                    if (sb)
                        points.Add(segmentsA[1].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsA[1]));

                    if (sc)
                        points.Add(segmentsB[0].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsB[0]));

                    if (sd)
                        points.Add(segmentsB[1].PointB);
                    else
                        this.roadSegments.RemoveAll(p => p.IsEqual(segmentsB[1]));

                    Intersection inter = new Intersection(points);
                    this.intersections.Add(inter);
                }
            }
            if (seg1 || seg2)
            {
                RoadSegment[] segments = this.patchSegment(segment, new RoadPoint((int)newPoint.x, (int)newPoint.z, segment));

                if (seg1 && seg2)
                {
                    Intersection inter = new Intersection(new List<RoadPoint> { segments[0].PointB, segments[1].PointB, newSegment.PointA, newSegmentOther.PointA });
                    this.intersections.Add(inter);
                }
                else if (seg1)
                {
                    Intersection inter = new Intersection(new List<RoadPoint> { segments[0].PointB, segments[1].PointB, newSegment.PointA });
                    this.intersections.Add(inter);
                }
                else if (seg2)
                {
                    Intersection inter = new Intersection(new List<RoadPoint> { segments[0].PointB, segments[1].PointB, newSegmentOther.PointA });
                    this.intersections.Add(inter);
                }
            }
        }

        private bool SegmentWithin(RoadSegment segment, float max)
        {
            foreach (RoadSegment seg in this.roadSegments)
            {
                bool amax = distPointSegment(seg.PointA, segment) < max;
                bool bmax = distPointSegment(seg.PointB, segment) < max;

                bool amin = MinPointDistance(seg, segment, max / 1.0f);

                if (amax || bmax || amin)
                    return true;
            }

            return false;
        }
        private float distPointSegment(RoadPoint P, RoadSegment S)
        {
            Vector2 p = new Vector2(P.point[0], P.point[1]);
            Vector2 sa = new Vector2(S.PointA.point[0], S.PointA.point[1]);
            Vector2 sb = new Vector2(S.PointB.point[0], S.PointB.point[1]);
            Vector2 v = new Vector2(S.PointB.point[0] - S.PointA.point[0], S.PointB.point[1] - S.PointA.point[1]);

            Vector2 w = new Vector2(P.point[0] - S.PointA.point[0], P.point[1] - S.PointA.point[1]);

            float c1 = Vector2.Dot(w, v);
            if (c1 <= 0)
                return Vector2.Distance(p, sa);

            float c2 = Vector2.Dot(v, v);
            if (c2 <= c1)
                return Vector2.Distance(p, sb);

            float b = c1 / c2;
            Vector2 Pb = sa + (v * b);
            return Vector2.Distance(p, Pb);
        }
        private bool MinPointDistance(RoadSegment a, RoadSegment b, float min)
        {
            Vector2 aPointA = new Vector2(a.PointA.point[0], a.PointA.point[1]);
            Vector2 bPointA = new Vector2(b.PointA.point[0], b.PointA.point[1]);
            if (Vector2.Distance(aPointA, bPointA) < min)
                return true;
            Vector2 bPointB = new Vector2(b.PointB.point[0], b.PointB.point[1]);
            if (Vector2.Distance(aPointA, bPointB) < min)
                return true;
            Vector2 aPointB = new Vector2(a.PointB.point[0], a.PointB.point[1]);
            if (Vector2.Distance(aPointB, bPointA) < min)
                return true;
            if (Vector2.Distance(aPointB, bPointB) < min)
                return true;

            return false;
        }

        private int segmentIntersection(RoadSegment segment, out Vector2 intersection, out RoadSegment other, RoadSegment skip)
        {
            Vector2 segmentPointA = new Vector2(segment.PointA.point[0], segment.PointA.point[1]);
            Vector2 segmentPointB = new Vector2(segment.PointB.point[0], segment.PointB.point[1]);

            intersection = Vector2.zero;
            other = null;

            Vector2 tmp = Vector2.zero;
            Vector2 interTmp = Vector3.zero;

            int count = 0;
            // foreach (RoadSegment seg in this.RoadSegments)
            for (int i = 0; i < this.roadSegments.Count; i++)
            {
                RoadSegment seg = this.roadSegments[i];
                Vector2 segPointA = new Vector2(seg.PointA.point[0], seg.PointA.point[1]);
                Vector2 segPointB = new Vector2(seg.PointB.point[0], seg.PointB.point[1]);
                if (seg.IsEqual(skip))
                    continue;
                else if (Vector2.Distance(segPointA, segmentPointA) < 0.01f || Vector2.Distance(segPointB, segmentPointB) < 0.01f)
                    continue;
                else if (Vector2.Distance(segPointA, segmentPointB) < 0.01f || Vector2.Distance(segPointB, segmentPointA) < 0.01f)
                    continue;
                else if (inter2Segments(segment, seg, out interTmp, out tmp) != 0)
                {
                    other = seg;
                    intersection = new Vector2(interTmp.x, interTmp.y);
                    count++;
                }
            }

            return count;
        }
        //http://geomalgorithms.com/a05-_intersect-1.html
        // intersect2D_2Segments(): find the 2D intersection of 2 finite segments
        //    Input:  two finite segments S1 and S2
        //    Output: *I0 = intersect point (when it exists)
        //            *I1 =  endpoint of intersect segment [I0,I1] (when it exists)
        //    Return: 0=disjoint (no intersect)
        //            1=intersect  in unique point I0
        //            2=overlap  in segment from I0 to I1
        int inter2Segments(RoadSegment S1, RoadSegment S2, out Vector2 I0, out Vector2 I1)
        {
            Vector2 S1PointA = new Vector2(S1.PointA.point[0], S1.PointA.point[1]);
            Vector2 S1PointB = new Vector2(S1.PointB.point[0], S1.PointB.point[1]);
            Vector2 S2PointA = new Vector2(S2.PointA.point[0], S2.PointA.point[1]);
            Vector2 S2PointB = new Vector2(S2.PointB.point[0], S2.PointB.point[1]);

            Vector2 u = S1PointB - S1PointA;
            Vector2 v = S2PointB - S2PointA;
            Vector2 w = S1PointA - S2PointA;
            float D = perp(u, v);

            I0 = Vector2.zero;
            I1 = Vector2.zero;

            // test if  they are parallel (includes either being a point)
            if (Mathf.Abs(D) < 0.01f)
            {           // S1 and S2 are parallel
                if (perp(u, w) != 0 || perp(v, w) != 0)
                {
                    return 0;                    // they are NOT collinear
                }
                // they are collinear or degenerate
                // check if they are degenerate  points
                float du = Vector2.Dot(u, u);
                float dv = Vector2.Dot(v, v);
                if (du == 0 && dv == 0)
                {            // both segments are points
                    if (S1.PointA.point != S2.PointA.point)         // they are distinct  points
                        return 0;
                    I0 = S1PointA;                 // they are the same point
                    return 1;
                }
                if (du == 0)
                {                     // S1 is a single point
                    if (inSegment(S1.PointA, S2) == 0)  // but is not in S2
                        return 0;
                    I0 = S1PointA;
                    return 1;
                }
                if (dv == 0)
                {                     // S2 a single point
                    if (inSegment(S2.PointA, S1) == 0)  // but is not in S1
                        return 0;
                    I0 = S2PointA;
                    return 1;
                }
                // they are collinear segments - get  overlap (or not)
                float t0, t1;                    // endpoints of S1 in eqn for S2
                Vector2 w2 = S1PointB - S2PointA;
                if (v.x != 0)
                {
                    t0 = w.x / v.x;
                    t1 = w2.x / v.x;
                }
                else
                {
                    t0 = w.y / v.y;
                    t1 = w2.y / v.y;
                }
                if (t0 > t1)
                {                   // must have t0 smaller than t1
                    float t = t0; t0 = t1; t1 = t;    // swap if not
                }
                if (t0 > 1 || t1 < 0)
                {
                    return 0;      // NO overlap
                }
                t0 = t0 < 0 ? 0 : t0;               // clip to min 0
                t1 = t1 > 1 ? 1 : t1;               // clip to max 1
                if (t0 == t1)
                {                  // intersect is a point
                    I0 = S2PointA + t0 * v;
                    return 1;
                }

                // they overlap in a valid subsegment
                I0 = S2PointA + t0 * v;
                I1 = S2PointA + t1 * v;
                return 2;
            }

            // the segments are skew and may intersect in a point
            // get the intersect parameter for S1
            float sI = perp(v, w) / D;
            if (sI < 0 || sI > 1)                // no intersect with S1
                return 0;

            // get the intersect parameter for S2
            float tI = perp(u, w) / D;
            if (tI < 0 || tI > 1)                // no intersect with S2
                return 0;

            Vector2 shit = S1PointA + sI * u;
            I0 = new Vector2((int)(shit.x+0.5f), (int)(shit.y + 0.5f));       // compute S1 intersect point;
            return 1;
        }

        int inSegment(RoadPoint P, RoadSegment S)
        {
            if (S.PointA.point[0] != S.PointB.point[0])
            {    // S is not  vertical
                if (S.PointA.point[0] <= P.point[0] && P.point[0] <= S.PointB.point[0])
                    return 1;
                if (S.PointA.point[0] >= P.point[0] && P.point[0] >= S.PointB.point[0])
                    return 1;
            }
            else
            {    // S is vertical, so test y  coordinate
                if (S.PointA.point[1] <= P.point[1] && P.point[1] <= S.PointB.point[1])
                    return 1;
                if (S.PointA.point[1] >= P.point[1] && P.point[1] >= S.PointB.point[1])
                    return 1;
            }
            return 0;
        }

        private float perp(Vector2 u, Vector2 v)
        {
            return ((u).x * (v).y - (u).y * (v).x);
        }

        private RoadSegment[] patchSegment(RoadSegment segment, RoadPoint newPoint)
        {
            this.roadSegments.RemoveAll(p => p.IsEqual(segment));

            RoadSegment left = new RoadSegment(segment.PointA, new RoadPoint(newPoint.point), segment.Level);
            RoadSegment right = new RoadSegment(segment.PointB, new RoadPoint(newPoint.point), segment.Level);

            this.roadSegments.Add(left);
            this.roadSegments.Add(right);

            return new RoadSegment[] { left, right };
        }

        public void InitRoadCentre(int loX, int loZ, int CityWidth, int CithHeight)
        {
            RoadPoint centre = new RoadPoint(loX, loZ);
            RoadPoint leftEnd = new RoadPoint(0, loZ);
            RoadPoint rightEnd = new RoadPoint(CityWidth, loZ);
            RoadPoint topEnd = new RoadPoint(loX, 0);
            RoadPoint bottomEnd = new RoadPoint(loX, CithHeight);

            RoadSegment rA = new RoadSegment(centre, leftEnd, 0);
            RoadSegment rB = new RoadSegment(centre, rightEnd, 0);
            RoadSegment rC = new RoadSegment(centre, topEnd, 0);
            RoadSegment rD = new RoadSegment(centre, bottomEnd, 0);

            this.roadSegments.AddRange(new RoadSegment[] { rA, rB, rC, rD });
            Intersection iA = new Intersection(new List<RoadPoint>() { rA.PointA, rB.PointA, rC.PointA, rD.PointA });
            this.intersections.Add(iA);
        }

    }
    public class RoadSegment
    {
        public RoadPoint PointA { get; private set; }
        public RoadPoint PointB { get; private set; }

        public int Level { get; private set; }

        public RoadSegment(RoadPoint a, RoadPoint b, int level)
        {
            this.PointA = new RoadPoint(a.point, this);
            this.PointB = new RoadPoint(b.point, this);

            this.Level = level;
        }

        public RoadPoint GetOther(RoadPoint main)
        {
            return this.PointA.Equals(main) ? this.PointB : this.PointA;
        }
        public float SegmentLength()
        {
            return Vector2.Distance(new Vector2(this.PointA.point[0], this.PointA.point[1]),new Vector2(this.PointB.point[0], this.PointB.point[1]));
        }
        public int[] GetLocation(bool first)
        {
            int[] returnThis = new int[3];
            if (first)
            {
                returnThis[0] = this.PointA.point[0];
                returnThis[1] = 0;
                returnThis[2] = this.PointA.point[1];
            }
            else
            {
                returnThis[0] = this.PointB.point[0];
                returnThis[1] = 0;
                returnThis[2] = this.PointB.point[1];
            }
            return returnThis;
        }
        public bool IsEqual(RoadSegment segment)
        {
            if (this.PointA.Equals(segment.PointA) && this.PointB.Equals(segment.PointB))
                return true;
            else if (this.PointA.Equals(segment.PointB) && this.PointB.Equals(segment.PointA))
                return true;

            return false;
        }
    }
    public class RoadPoint
    {
        public int[] point = new int[2];
        public RoadSegment pSegment { get; set; }

        public RoadPoint() { }
        public RoadPoint(int x, int y, RoadSegment segment = null)
        {
            this.point[0] = x;
            this.point[1] = y;
            this.pSegment = segment;
        }
        public RoadPoint(int[] point, RoadSegment segment = null)
        {
            this.point[0] = point[0];
            this.point[1] = point[1];
            this.pSegment = segment;
        }

        public override bool Equals(object obj)
        {
            RoadPoint other = obj as RoadPoint;
            if(other.point[0] == this.point[0] && other.point[1] == this.point[1])
                return true;
            return false;
        }
    }
    public struct Intersection
    {
        public List<RoadPoint> points;

        public Intersection(List<RoadPoint> points)
        {
            this.points = points;
        }

        public bool IsThisOne(Intersection inter)
        {
            int c = 0;
            foreach(RoadPoint p in inter.points)
            {
                if (this.points.Exists(f => f == p))
                    c++;
            }
            if (c == this.points.Count && c == inter.points.Count)
                return true;
            return false;
        }
    }
}