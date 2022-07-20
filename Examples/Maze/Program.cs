namespace Maze;

using System;
using System.Collections.Generic;
using Substrate;

class Program {
    static void Main(string[] args) {
        if(args.Length < 1) {
            Console.WriteLine("You must specify a target directory");
            return;
        }
        string dest = args[0];

        AnvilWorld world = AnvilWorld.Open("F:\\Minecraft\\test");
        BlockManager bm = world.GetBlockManager();

        bm.AutoLight = false;

        Grid grid = new Grid();
        grid.BuildInit(bm);

        Generator gen = new Generator();
        List<Generator.Edge> edges = gen.Generate();

        foreach(Generator.Edge e in edges) {
            int x1;
            int y1;
            int z1;
            gen.UnIndex(e.node1, out x1, out y1, out z1);

            int x2;
            int y2;
            int z2;
            gen.UnIndex(e.node2, out x2, out y2, out z2);

            grid.LinkRooms(bm, x1, y1, z1, x2, y2, z2);
        }

        // Entrance Room
        grid.BuildRoom(bm, 2, 5, 2);
        grid.LinkRooms(bm, 2, 5, 2, 1, 5, 2);
        grid.LinkRooms(bm, 2, 5, 2, 3, 5, 2);
        grid.LinkRooms(bm, 2, 5, 2, 2, 5, 1);
        grid.LinkRooms(bm, 2, 5, 2, 2, 5, 3);
        grid.LinkRooms(bm, 2, 4, 2, 2, 5, 2);

        // Exit Room
        grid.BuildRoom(bm, 2, -1, 2);
        grid.LinkRooms(bm, 2, -1, 2, 2, 0, 2);
        grid.AddPrize(bm, 2, -1, 2);

        Console.WriteLine("Relight Chunks");

        RegionChunkManager cm = world.GetChunkManager();
        cm.RelightDirtyChunks();

        world.Save();
    }
}

class Grid {
    int originx;
    int originy;
    int originz;

    int xlen;
    int ylen;
    int zlen;

    int cellxlen;
    int cellylen;
    int cellzlen;
    int wallxwidth;
    int wallywidth;
    int wallzwidth;

    public Grid() {
        this.originx = 0;
        this.originy = 27;
        this.originz = 0;

        this.xlen = 5;
        this.ylen = 5;
        this.zlen = 5;

        this.cellxlen = 5;
        this.cellylen = 5;
        this.cellzlen = 5;
        this.wallxwidth = 2;
        this.wallywidth = 2;
        this.wallzwidth = 2;
    }

    public void BuildInit(BlockManager bm) {
        for(int xi = 0; xi < this.xlen; xi++) {
            for(int yi = 0; yi < this.ylen; yi++) {
                for(int zi = 0; zi < this.zlen; zi++) {
                    BuildRoom(bm, xi, yi, zi);
                }
            }
        }
    }

    public void BuildRoom(BlockManager bm, int x, int y, int z) {
        int ox = this.originx + (this.cellxlen + this.wallxwidth) * x;
        int oy = this.originy + (this.cellylen + this.wallywidth) * y;
        int oz = this.originz + (this.cellzlen + this.wallzwidth) * z;

        // Hollow out room
        for(int xi = 0; xi < this.cellxlen; xi++) {
            int xx = ox + this.wallxwidth + xi;
            for(int zi = 0; zi < this.cellzlen; zi++) {
                int zz = oz + this.wallzwidth + zi;
                for(int yi = 0; yi < this.cellylen; yi++) {
                    int yy = oy + this.wallywidth + yi;
                    bm.SetID(xx, yy, zz, (int) BlockType.AIR);
                }
            }
        }

        // Build walls
        for(int xi = 0; xi < this.cellxlen + 2 * this.wallxwidth; xi++) {
            for(int zi = 0; zi < this.cellzlen + 2 * this.wallzwidth; zi++) {
                for(int yi = 0; yi < this.wallywidth; yi++) {
                    bm.SetID(ox + xi, oy + yi, oz + zi, (int) BlockType.BEDROCK);
                    bm.SetID(ox + xi, oy + yi + this.cellylen + this.wallywidth, oz + zi, (int) BlockType.BEDROCK);
                }
            }
        }

        for(int xi = 0; xi < this.cellxlen + 2 * this.wallxwidth; xi++) {
            for(int zi = 0; zi < this.wallzwidth; zi++) {
                for(int yi = 0; yi < this.cellylen + 2 * this.wallywidth; yi++) {
                    bm.SetID(ox + xi, oy + yi, oz + zi, (int) BlockType.BEDROCK);
                    bm.SetID(ox + xi, oy + yi, oz + zi + this.cellzlen + this.wallzwidth, (int) BlockType.BEDROCK);
                }
            }
        }

        for(int xi = 0; xi < this.wallxwidth; xi++) {
            for(int zi = 0; zi < this.cellzlen + 2 * this.wallzwidth; zi++) {
                for(int yi = 0; yi < this.cellylen + 2 * this.wallywidth; yi++) {
                    bm.SetID(ox + xi, oy + yi, oz + zi, (int) BlockType.BEDROCK);
                    bm.SetID(ox + xi + this.cellxlen + this.wallxwidth, oy + yi, oz + zi, (int) BlockType.BEDROCK);
                }
            }
        }

        // Torchlight
        bm.SetID(ox + this.wallxwidth, oy + this.wallywidth + 2, oz + this.wallzwidth + 1, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth, oy + this.wallywidth + 2, oz + this.wallzwidth + this.cellzlen - 2, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + this.cellxlen - 1, oy + this.wallywidth + 2, oz + this.wallzwidth + 1, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + this.cellxlen - 1, oy + this.wallywidth + 2, oz + this.wallzwidth + this.cellzlen - 2, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + 1, oy + this.wallywidth + 2, oz + this.wallzwidth, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + this.cellxlen - 2, oy + this.wallywidth + 2, oz + this.wallzwidth, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + 1, oy + this.wallywidth + 2, oz + this.wallzwidth + this.cellzlen - 1, (int) BlockType.TORCH);
        bm.SetID(ox + this.wallxwidth + this.cellxlen - 2, oy + this.wallywidth + 2, oz + this.wallzwidth + this.cellzlen - 1, (int) BlockType.TORCH);
    }

    public void LinkRooms(BlockManager bm, int x1, int y1, int z1, int x2, int y2, int z2) {
        int xx = this.originx + (this.cellxlen + this.wallxwidth) * x1;
        int yy = this.originy + (this.cellylen + this.wallywidth) * y1;
        int zz = this.originz + (this.cellzlen + this.wallzwidth) * z1;

        if(x1 != x2) {
            xx = this.originx + (this.cellxlen + this.wallxwidth) * Math.Max(x1, x2);
            for(int xi = 0; xi < this.wallxwidth; xi++) {
                int zc = zz + this.wallzwidth + (this.cellzlen / 2);
                int yb = yy + this.wallywidth;
                bm.SetID(xx + xi, yb, zc - 1, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb, zc, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb, zc + 1, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb + 1, zc - 1, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb + 1, zc, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb + 1, zc + 1, (int) BlockType.AIR);
                bm.SetID(xx + xi, yb + 2, zc, (int) BlockType.AIR);
            }
        } else if(z1 != z2) {
            zz = this.originz + (this.cellzlen + this.wallzwidth) * Math.Max(z1, z2);
            for(int zi = 0; zi < this.wallxwidth; zi++) {
                int xc = xx + this.wallxwidth + (this.cellxlen / 2);
                int yb = yy + this.wallywidth;
                bm.SetID(xc - 1, yb, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc, yb, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc + 1, yb, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc - 1, yb + 1, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc, yb + 1, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc + 1, yb + 1, zz + zi, (int) BlockType.AIR);
                bm.SetID(xc, yb + 2, zz + zi, (int) BlockType.AIR);
            }
        } else if(y1 != y2) {
            yy = this.originy + (this.cellylen + this.wallywidth) * Math.Max(y1, y2);
            for(int yi = 0 - this.cellylen + 1; yi < this.wallywidth + 1; yi++) {
                int xc = xx + this.wallxwidth + (this.cellxlen / 2);
                int zc = zz + this.wallzwidth + (this.cellzlen / 2);

                bm.SetID(xc, yy + yi, zc, (int) BlockType.BEDROCK);
                bm.SetID(xc - 1, yy + yi, zc, (int) BlockType.LADDER);
                bm.SetData(xc - 1, yy + yi, zc, 4);
                bm.SetID(xc + 1, yy + yi, zc, (int) BlockType.LADDER);
                bm.SetData(xc + 1, yy + yi, zc, 5);
                bm.SetID(xc, yy + yi, zc - 1, (int) BlockType.LADDER);
                bm.SetData(xc, yy + yi, zc - 1, 2);
                bm.SetID(xc, yy + yi, zc + 1, (int) BlockType.LADDER);
                bm.SetData(xc, yy + yi, zc + 1, 3);
            }
        }
    }

    public void AddPrize(BlockManager bm, int x, int y, int z) {
        int ox = this.originx + (this.cellxlen + this.wallxwidth) * x + this.wallxwidth;
        int oy = this.originy + (this.cellylen + this.wallywidth) * y + this.wallywidth;
        int oz = this.originz + (this.cellzlen + this.wallzwidth) * z + this.wallzwidth;

        Random rand = new Random();
        for(int xi = 0; xi < this.cellxlen; xi++) {
            for(int zi = 0; zi < this.cellzlen; zi++) {
                if(rand.NextDouble() < 0.1) {
                    bm.SetID(ox + xi, oy, oz + zi, (int) BlockType.DIAMOND_BLOCK);
                }
            }
        }
    }
}

class Generator {
    public struct Edge {
        public int node1;
        public int node2;

        public Edge(int n1, int n2) {
            this.node1 = n1;
            this.node2 = n2;
        }
    }

    int xlen;
    int ylen;
    int zlen;

    List<Edge> _edges;
    int[] _cells;

    public Generator() {
        this.xlen = 5;
        this.ylen = 5;
        this.zlen = 5;

        this._edges = new List<Edge>();
        this._cells = new int[this.xlen * this.zlen * this.ylen];

        for(int x = 0; x < this.xlen; x++) {
            for(int z = 0; z < this.zlen; z++) {
                for(int y = 0; y < this.ylen; y++) {
                    int n1 = Index(x, y, z);
                    this._cells[n1] = n1;
                }
            }
        }

        for(int x = 0; x < this.xlen - 1; x++) {
            for(int z = 0; z < this.zlen; z++) {
                for(int y = 0; y < this.ylen; y++) {
                    int n1 = Index(x, y, z);
                    int n2 = Index(x + 1, y, z);
                    this._edges.Add(new Edge(n1, n2));
                }
            }
        }

        for(int x = 0; x < this.xlen; x++) {
            for(int z = 0; z < this.zlen - 1; z++) {
                for(int y = 0; y < this.ylen; y++) {
                    int n1 = Index(x, y, z);
                    int n2 = Index(x, y, z + 1);
                    this._edges.Add(new Edge(n1, n2));
                }
            }
        }

        for(int x = 0; x < this.xlen; x++) {
            for(int z = 0; z < this.zlen; z++) {
                for(int y = 0; y < this.ylen - 1; y++) {
                    int n1 = Index(x, y, z);
                    int n2 = Index(x, y + 1, z);
                    this._edges.Add(new Edge(n1, n2));
                }
            }
        }
    }

    public List<Edge> Generate() {
        Random rand = new Random();

        List<Edge> passages = new List<Edge>();

        // Randomize edges
        Queue<Edge> redges = new Queue<Edge>();
        while(this._edges.Count > 0) {
            int index = rand.Next(this._edges.Count);
            Edge e = this._edges[index];
            this._edges.RemoveAt(index);
            redges.Enqueue(e);
        }

        while(redges.Count > 0) {
            Edge e = redges.Dequeue();

            if(this._cells[e.node1] == this._cells[e.node2]) {
                continue;
            }

            passages.Add(e);

            int n1 = this._cells[e.node2];
            int n2 = this._cells[e.node1];
            for(int i = 0; i < this._cells.Length; i++) {
                if(this._cells[i] == n2) {
                    this._cells[i] = n1;
                }
            }
        }

        return passages;
    }

    public int Index(int x, int y, int z) {
        return (x * this.zlen + z) * this.ylen + y;
    }

    public void UnIndex(int index, out int x, out int y, out int z) {
        x = index / (this.zlen * this.ylen);
        int xstr = index - (x * this.zlen * this.ylen);
        z = xstr / this.ylen;
        int ystr = xstr - (z * this.ylen);
        y = ystr;
    }
}
