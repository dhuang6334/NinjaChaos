using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using Random = UnityEngine.Random;
public class MapGen : MonoBehaviour
{

    public class Matrix
    {
        public readonly List<CellData> cells = new List<CellData>(); 

        Vector2Int size;
        public Matrix(Vector2Int _size)
        {
            this.size = _size;

            for (int y = 0; y < this.size.y; y++)
            {
                for (int x = 0; x < this.size.x; x++)
                {
                    CellData cell = new CellData()
                    {
                        worldLocation = new Vector3Int(x, y, 0)
                    };
                    //cell.
                    //cell.tileKey = 1;       // NOTE Cells automatically start as wall/ No need to set their attributes again
                    //cell.walkable = false;
                    cells.Add(cell);
                }
            }

            /*
            void debug()
            {
            for (int i = 0; i < matrix.Count; i++)
                {
                    Debug.Log(i);
                } 
            }
            debug();
            */
        }
        
        
        int CoordToInd(int x, int y)
            {
                if (x < this.size.x && y < this.size.y && x >= 0 && y >= 0)
                    return x + (y * this.size.x);
                else
                    Debug.LogError("Index Out of Range of Matrix");  //REVIEW does this work?
                    return -1;
            }
        public CellData this[Vector2Int pos] 
            {
                get => this.cells[CoordToInd(pos.x, pos.y)];
                set => this.cells[CoordToInd(pos.x, pos.y)] = value;
            }
        
        public void Load()
        {
            // load whole matrix by loading all cells
            foreach (CellData cell in cells)
            {
                cell.Load();
            }
        }
    }
    public class CellData
    {
        public Vector2Int cellLocation { get; set; }

        public Vector3Int worldLocation { get; set; }

        public int tileKey { get; set; }

        public Tilemap tilemap { get; set; } // gets/sets the tilemap that the cell is from

        //public Sprite sprite { get; set; }  REVIEW is this necessary?

        //for Map generation
        public bool walkable { get; set; }

        public int roomFlag { get; set;}

        public CellData() // default cell is a "wall"
        {
            this.ToWall();
        }
        public void ToWall()
        {
            this.tileKey = 1;
            this.walkable = false;
            this.roomFlag = -1;
        }

        public void ToFloor()
        {
            this.tileKey = 0;
            this.walkable = true;
            this.roomFlag = 0; //default flag is 0 (changes during map generation)
        }

        public void To(string name)
        {
            // TODO add other tiles to change to with parameters
        }

        public void Load()
        {
            // TODO load cell 
        }
    }

    public class Room  // only the blank space in a room
    {
        public Vector2Int pos { get; set; }
        public Vector2Int size { get; set; }

        public static int rooms = 1;
        
        public void PlaceRoom()
        {
            for (int y = 0; y < size.y; y++)  // doesnt matter in this case if y or x is outside loop
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2Int deltaRoom = new Vector2Int(x, y);
                    matrix[pos + deltaRoom].ToFloor();
                    matrix[pos + deltaRoom].roomFlag = rooms;
                }
            }

            rooms += 1;
        }

        /* // REVIEW do I need this constructor?
                public Room(Vector2Int _pos, Vector2Int _size)
                {
                    this.pos = _pos;
                    this.size = _size;
                }
        */ 
    }
// ANCHOR Map Size
    Vector2Int mapBorder = new Vector2Int(3, 3);
    Vector2Int mapSize = new Vector2Int(64, 64); 

    int hallBorder = 1;
    

    //private static List<List<CellData>> lvls = new List<List<CellData>>();
    //private static List<List<int>> matrix = new List<List<int>>();
    
    private static List<Matrix> lvls = new List<Matrix>();
    private static Matrix matrix;
    private Grid grid;
    
    [SerializeField] 
    private List<TileBase> tileBases;
    [SerializeField]
    private List<int> tileKeys;
    private Dictionary<int, TileBase> tileDict;

    public Dictionary<string, Tilemap> tileMaps = new Dictionary<string, Tilemap>();


    

    // Basic Tilemap Constructor Function (no colliders)
    private void CreateTilemap(string tilemapName, float AnchorX, float AnchorY, string sortingLayer, string type = "None")
    {
        GameObject _gameobject = new GameObject(tilemapName);
        Tilemap _tilemap = _gameobject.AddComponent<Tilemap>();
        TilemapRenderer _renderer = _gameobject.AddComponent<TilemapRenderer>();
        
        if (type == "Collider")
        {
            TilemapCollider2D _tilemapCollider = _gameobject.AddComponent<TilemapCollider2D>();
        Rigidbody2D _rb2d = _gameobject.AddComponent<Rigidbody2D>();
        CompositeCollider2D _compCollider = _gameobject.AddComponent<CompositeCollider2D>();
        
        _tilemapCollider.usedByComposite = true;
        _rb2d.gravityScale = 0f;
        _rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        

        _tilemap.tileAnchor = new Vector3(AnchorX, AnchorY, 0);
        _gameobject.transform.SetParent(grid.transform);
        _renderer.sortingLayerName = sortingLayer;

        

        tileMaps.Add(tilemapName, _tilemap);
        
    }

    
    void RandMatrix()
    {
        
        
        matrix = new Matrix(mapSize);
        /*
        Matrix mat = new Matrix(mapSize);
        Matrix mat2 = new Matrix(mapSize);
        CellData data = new CellData();
        data.cellLocation = new Vector3Int(1, 0, 1);
        CellData data2 = new CellData();
        data2.cellLocation = new Vector3Int(1, 5, 1);
        // matrix class debugging
        mat[0, 0] = data;
        mat2[0, 0] = data2;
        Debug.Log(mat[0, 1].cellLocation);
        Debug.Log(mat[0,0].cellLocation);
        Debug.Log(mat2[0, 1].cellLocation);
        Debug.Log(mat2[0,0].cellLocation);
        */

        /*
        int width = mapSize.x; 
        int height = mapSize.y;
        matrix = new List<List<int>>();
        for (int y = 0; y < height; y++)
        {
            List<int> row = new List<int>();
            for (int x = 0; x < width; x++)
            {
                row.Add(1);
            }
            matrix.Add(row);
        }
        */
        GenRooms();
    }
    
    // ANCHOR ROOMS
    void GenRooms()
    {
        int fmax = 5; // max # of failures
        int rmax = 4; // max # of rooms
        int maxWidth = 10;
        int maxHeight = 10;
        int minWidth = 3;
        int minHeight = 3;
        /*
        while ( fmax > 0 || rmax > 0)  // Future me thinking shouldn't this be && instead? to make it the same?
        {
            Room newRoom = RandRoom(minWidth, minHeight, maxWidth, maxHeight);
            if (RandPlaceRoom(newRoom))
            {
                rmax -= 1;
            } else {
                fmax -= 1;
                if (newRoom.size.x > newRoom.size.y)
                    maxWidth = Math.Max(minWidth, maxWidth - 1);
                else
                    maxHeight = Math.Max(minHeight, maxHeight - 1);

            }
        }
        */
        for (int i = 0; i < rmax; i++)
        {
            Room newRoom = RandRoom(minWidth, minHeight, maxWidth, maxHeight);
            if (!RandPlaceRoom(newRoom))
            {
                fmax -= 1;
                if (fmax <= 0)
                {
                    break;
                }
                if (newRoom.size.x > newRoom.size.y)
                    maxWidth = Math.Max(minWidth, maxWidth - 1);
                else
                    maxHeight = Math.Max(minHeight, maxHeight - 1);

            }
        }
        
        
    }
    Room RandRoom(int _minWidth, int _minHeight, int _maxWidth, int _maxHeight)
    {
        int width = Random.Range(_minWidth, _maxWidth);
        int height = Random.Range(_minHeight, _maxHeight);
        
        Room room = new Room()
        {
            pos = new Vector2Int(0,0), // default (0,0)  
            size = new Vector2Int(width, height)
        };
        return room;
    }
    bool RandPlaceRoom(Room room)
        {
            List<Vector2Int> cands = PossibleLocationsFor(room);
            
            if (cands.Count == 0)
                return false;
            else
            {
            Vector2Int cand = cands[Random.Range(0, cands.Count)];
            room.pos = cand;
            
            room.PlaceRoom();
            
            return true; 
            }
            
        }
    List<Vector2Int> PossibleLocationsFor(Room room)  // returns vector2Ints
    {
        List<Vector2Int> cands = new List<Vector2Int>();

        int width = mapSize.x - room.size.x - mapBorder.x; 
        int height = mapSize.y - room.size.y - mapBorder.y;
        for (int y = mapBorder.y; y < height; y++)  // doesnt matter in this case if y or x is outside loop
        {
            for (int x = mapBorder.x; x < width; x++) 
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (DoesRoomFit(room, pos))
                    cands.Add(pos);
            }
        }

        return cands;
    }
    bool DoesRoomFit(Room room, Vector2Int pos)
    {
        
        for (int y = -1; y < room.size.y + 1; y++)  // doesnt matter in this case if y or x is outside loop
            {
                for (int x = -1; x < room.size.x + 1; x++) // checks one less and one more to account for walls of room
                {
                    
                    if (Walkable(pos + new Vector2Int(x, y)))
                        return false;
                }
            }
        
        return true;
    }
    
    bool Walkable(Vector2Int pos)
    {
        try
        {
            /*
            for (int y = 0; y < (2 * hallBorder) + 1; y++)  // doesnt matter in this case if y or x is outside loop
            {
                for (int x = 0; x < (2 * hallBorder) + 1; x++)
                {
                    Vector2Int deltaRoom = new Vector2Int(x, y);
                    Vector2Int deltaPoint = new Vector2Int(-hallBorder, -hallBorder);
                    matrix[pos + deltaRoom + deltaPoint].walkable;
                }
            }*/
            if (matrix[pos].walkable)
                return true;
            else
                return false;
        } catch {
            return false;
        }
        
    }

    bool WithinMap(Vector2Int pos)
    {
        return mapBorder.x <= pos.x && pos.x < mapSize.x - mapBorder.x && mapBorder.y <= pos.y && pos.y < mapSize.y - mapBorder.y;
    }
    //ANCHOR Carve Signatures
    List<int> crv_sig = new List<int>()
        {
            0b11111111, 0b11010110, 0b01111100, 0b10110011, 0b11101001
        };
    List<int> crv_msk = new List<int>()
    {
        0b0       , 0b00001001, 0b00000011, 0b00001100, 0b00000110
    };
    List<Vector2Int> delta = new List<Vector2Int>() // TODO "Hallway borders" (delta + (2*(hallway border size))
        {
            Vector2Int.left,  //L
            Vector2Int.right,   //R
            Vector2Int.up,   //U
            Vector2Int.down,  //D
            Vector2Int.up + Vector2Int.right,   //TR
            Vector2Int.down + Vector2Int.right,  //BR
            Vector2Int.down + Vector2Int.left, //BL
            Vector2Int.up + Vector2Int.left   //TL
        };

    int GetSig(Vector2Int pos)
    {
        int sig = 0b0;
        
        foreach (Vector2Int delta_pos in delta) 
        {
            sig = sig << 1;
            if (Walkable(pos + delta_pos))
                sig += 0;
            else
                sig += 1;
        }

        return sig;
    }
    // TODO Procedural Gen
    void MazeWorm()
    {
        List<Vector2Int> cands = new List<Vector2Int>();
        do
        {
            cands = new List<Vector2Int>();

            int width = mapSize.x; 
            int height = mapSize.y;
            
            foreach (CellData cell in matrix.cells)
            {
                Vector2Int pos = (Vector2Int) cell.worldLocation;
                if (!Walkable(pos) && WithinMap(pos) && GetSig(pos)==0b11111111) // Start carving at a wall surrounded by walls
                    cands.Add(pos);
            }

            if (cands.Count > 0)
                {
                    Vector2Int cand = cands[Random.Range(0, cands.Count)];
                    CarveFrom(cand);
                }
        } while (cands.Count > 1);
        
    }
    
    

    /* // REVIEW 2.5D Walls 
    void PrettyWalls()
    {
        foreach (CellData cell in matrix.cells)
        {
            if (Walkable((Vector2Int) cell.worldLocation) && !Walkable((Vector2Int) cell.worldLocation + Vector2Int.up))
            {
                cell.tileKey = 2;
            }
        }
    }
    */

    void CarveFrom(Vector2Int pos) // FIXME Carving for extended hallway borders fix!! carving does not work right now
    {
        
        int dir = Random.Range(0, 4);
        int steps = 0;

        while (dir != 7) // NOTE dir 7 is simply the indicator that there are no cands (the #7 is meaningless in this context)
        {
            //delta.ConvertAll(new Converter<Vector2Int, Vector2Int>()); experimenting on C# list mapping
            
            for (int y = 0; y < (2 * hallBorder) + 1; y++)  // doesnt matter in this case if y or x is outside loop
            {
                for (int x = 0; x < (2 * hallBorder) + 1; x++)
                {
                    Vector2Int deltaRoom = new Vector2Int(x, y);
                    Vector2Int deltaPoint = new Vector2Int(-hallBorder, -hallBorder);
                    matrix[pos + deltaRoom + deltaPoint].ToFloor();//FIXME add overload parameter for changing roomFlag of cell
                }
            }
            //matrix[pos].ToFloor(); 
            

            Vector2Int change(int i) => delta[i] * (1 + hallBorder);
            bool newCarvable(Vector2Int pos)
            {
                for (int i = -hallBorder; i <= hallBorder; i++)
                {
                    Vector2Int newDelta = new Vector2Int(i, hallBorder);
                    if (Carvable(pos + newDelta))
                        return true;
                }
                for (int i = -hallBorder + 1; i <= hallBorder - 1; i++)
                {
                    Vector2Int newDelta = new Vector2Int(-hallBorder, i);
                    if (Carvable(pos + newDelta))
                        return true;
                    newDelta = new Vector2Int(hallBorder, i);
                    if (Carvable(pos + newDelta))
                        return true;
                }
                for (int i = -hallBorder; i <= hallBorder; i++)
                {
                    Vector2Int newDelta = new Vector2Int(i, -hallBorder);
                    if (Carvable(pos + newDelta))
                        return true;
                }
                return false;
            }

            
            if (!Carvable(pos + delta[dir]) || (Random.Range(0,100) < 50 && steps > 2)) // 50% chance to randomly change direction only when it has moved more than 2 steps
            {
                steps = 0;
                List<int> cands = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    if (Carvable(pos + delta[i]))
                    {
                        cands.Add(i);
                    }
                }
                if (cands.Count == 0)
                {
                    dir = 7;
                }
                else
                {
                    dir = cands[Random.Range(0, cands.Count)];
                }
            }
            pos += delta[dir];
            steps += 1;
        }

    }

    /* REVIEW Prob Unneccessary
    void CarveMatrixAt(Vector2Int pos)
    {
        matrix[pos].ToFloor();
    }
    void FillMatrixAt(Vector2Int pos)
    {
        matrix[pos.y][pos.x] = 1;
    }
    */
    public bool Carvable(Vector2Int pos)
    {
        if (WithinMap(pos) && !Walkable(pos))
        {
            for (int i = 0; i < crv_sig.Count; i++)
            {
                if (SigMatch(GetSig(pos), crv_sig[i], crv_msk[i]))
                    return true;
            }
        }

        return false;
    }

    bool SigMatch(int sig, int match, int mask = 0)
    {
        return (sig|mask) == (match|mask);
    }


    public void SetTiles()
    {
        foreach (CellData cell in matrix.cells)
        {
            Tilemap _tilemap = cell.walkable ? tileMaps["Floor"] : tileMaps["Walls"];

            _tilemap.SetTile(cell.worldLocation, tileDict[cell.tileKey]);
            cell.tilemap = _tilemap;
        }
    }

    public void ClearAllTiles()
    {
        foreach (Tilemap tilemap in tileMaps.Values)
        {
            tilemap.ClearAllTiles();
        }
    }
    // Cave Cell Data/ Matrix
    void SaveDataLvl(int lvl = -1)
    {
        
        if (0 <= lvl && lvl < lvls.Count)
        {
            lvls[lvl] = matrix;
        } else if (lvl == -1) {
            lvls.Add(matrix);
        }
    }

    void DebugCarvable()
    {
        
        int width = mapSize.x; 
        int height = mapSize.y;
        List<List<int>> carvable = new List<List<int>>();
        for (int y = 0; y < height; y++)
        {
            List<int> row = new List<int>();
            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                row.Add(Carvable(pos) ? 1 : 0);
                if (Carvable(pos))
                {
                    Vector3Int pos3int = Vector3Int.FloorToInt(new Vector3(pos.x, pos.y, 0));
                    Color tileColor = tileMaps["Walls"].GetColor(pos3int);
                    tileMaps["Walls"].SetColor(pos3int , new Color(tileColor.r, tileColor.g, tileColor.b, 0.5f));
                }
                    
            }
            carvable.Add(row);
        }
        /*  PRINT values of carvable matrix
        foreach (List<int> row in carvable.Reverse<List<int>>())
            Debug.Log(string.Join("     ", row));
        */

    }

    /* REVIEW Prob unneccessary
    void BreakWall(Vector2Int worldPos)
    {
        Vector3Int worldPos3int = new Vector3Int(worldPos.x, worldPos.y, 0);
        Vector3Int cellPos = tileMaps["Walls"].WorldToCell(worldPos3int);
        tileMaps["Walls"].SetTile(cellPos, null);
        
        cellPos = tileMaps["Floor"].WorldToCell(worldPos3int);
        tileMaps["Floor"].SetTile(cellPos, tileDict[0]);

        matrix[worldPos.y][worldPos.x] = 0;
    }
    */


    // ANCHOR Game Loop
    private void Awake() 
    {
        grid = GetComponent<Grid>();
        tileDict = Enumerable.Range(0, tileKeys.Count).ToDictionary(i => tileKeys[i], i => tileBases[i]);
        
        CreateTilemap("Floor", 0.5f, 0.5f, "Background");
        CreateTilemap("Walls", 0.5f, 0.5f, "Foreground", "Collider");
        
        mapSize += mapBorder * 2; // account for border
        //matrix = new Matrix(mapSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        RandMatrix();
        MazeWorm();
        //PrettyWalls(); 

        //DEBUG
        Room room = new Room()
        {
            pos = new Vector2Int(2,2), 
            size = new Vector2Int(4, 4)
        };
        room.PlaceRoom();

        SetTiles();

        /* // old matrix debug values
        foreach (List<int> row in matrix.Reverse<List<List<int>>>())
            Debug.Log(string.Join("     ", row));
        Debug.Log(Convert.ToString(GetSig(new Vector2Int(1,2)), 2)); // debug getsig func
        */
        
        //DebugCarvable();
        
        /* // Cell rooms debugging
        foreach (CellData cell in matrix.cells)
            if (cell.walkable)
                Debug.Log(cell.roomFlag);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
