#include "graph.h"

using namespace std;
graph::graph():vertexNum(0), ribNum(0) {}


graph::~graph() {}

void graph::read(fstream& file)
{

	if (!file.is_open())
		throw "graph::read::file not opened\n";

	file >> vertexNum >> ribNum;

	G.resize(vertexNum);
	lenth.resize(vertexNum);

	float weight;
	for (int i(0), ver1, ver2; i < ribNum; i++)
	{

		file >> ver1 >> ver2 >> weight;
		if (ver1 > G.size() || ver2 > G.size())
			throw "graph::read::graph overflow\n";

		G[ver1 - 1].push_back(ver2 - 1);
		G[ver2 - 1].push_back(ver1 - 1);

		lenth[ver1 - 1].push_back(weight);
		lenth[ver2 - 1].push_back(weight);
	}
}

void graph::out()
{
	stack<int> vert;
	vector<int> color;
	for (int i(0); i < vertexNum; i++) color.push_back(0);

	bool traversed = false;
	vert.push(0);
	int currentVert;
	while (vert.size())
	{
		currentVert = vert.top();
		vert.pop();
		color[currentVert] = 2;

		cout << currentVert + 1 << " ";

		for (int i(0), size(G[currentVert].size()); i < size; i++)
		{
			if (color[G[currentVert][i]] == 0)
			{
				vert.push(G[currentVert][i]);
				color[G[currentVert][i]] = 1;
			}
		}
	}

	cout << std::endl;
}


vector<edge> graph::spintree_v1()
{
	//алгоритм прима
	const int start = 0;
	vector<edge> tree;

	vector<int> intree;
	intree.resize(vertexNum);
	for (int i(0); i < vertexNum; i++)
		intree[i] = 0;

	priority_queue<edge, vector<edge>> T;
	//add ribs incidents with start
	intree[start] = 1;
	for (int i(0), size(G[start].size()); i < size; i++)
		T.push(edge(start, G[start][i], 0));

	for (edge v; !T.empty(); )
	{
		v = T.top();
		T.pop();
		if (!intree[v.b])
		{
			intree[v.b]++;
			tree.push_back(v);
			for (int i(0), size(G[v.b].size()); i < size; i++)
				if (!intree[G[v.b][i]])
					T.push(edge(v.b, G[v.b][i], 0));
		}
	}

	return tree;
}

vector<edge> graph::spintree()
{
	//алгоритм прима
	const int start = 0;
	vector<edge> tree;

	vector<int> intree;
	intree.resize(vertexNum);
	for (int i(0); i < vertexNum; i++)
		intree[i] = 0;

	priority_queue<edge, vector<edge>, greater<edge>> T;
	//add ribs incidents with start
	intree[start] = 1; 
	for (int i(0), size(G[start].size()); i < size; i++)
		T.push(edge(start, G[start][i], lenth[start][i]));

	for (edge v; !T.empty(); )
	{
		v = T.top();
		T.pop();
		if (!intree[v.b])
		{
			intree[v.b]++;
			tree.push_back(v);
			for (int i(0), size(G[v.b].size()); i < size; i++)
				if (!intree[G[v.b][i]])
					T.push(edge(v.b, G[v.b][i], lenth[v.b][i]));
		}
	}

	return tree;
}

vector<edge> graph::spintreeKraskal()
{
	//алгоритм краскала
	const int start = 0;
	vector<edge> tree, Ribs;

	for (int i(0) ; i < this->vertexNum; i++)
	{
		for (int j(0); j < G[i].size(); j++)
		{
			Ribs.push_back(edge(i, G[i][j], this->lenth[i][j]));
		}
	}

	for (int i(0); i < Ribs.size(); i++)
	{
		for (int j(i + 1); j < Ribs.size(); j++)
		{
			if (Ribs[i] == Ribs[j])
			{
				Ribs.erase(Ribs.begin() + j);
				j--;
			}
		}
	}

	sort(Ribs.begin(), Ribs.end());

	//std::unique(Ribs.begin(), Ribs.end(), [](const edge& left, const edge& right) {return (left.a == right.a && left.b == right.b) || (left.a == right.b && left.b == right.a); });

	for (int i(0); i < Ribs.size(); i++)
	{
		bool contain = false;

		queue<int> BFS;
		vector<int> color;
		color.resize(this->vertexNum);
		for (int i(0); i < color.size(); i++) color[i] = 0;
		BFS.push(Ribs[i].a);
		int current;
		while (BFS.size())
		{
			current = BFS.front();
			BFS.pop();
			color[current] = 2;
			if (current == Ribs[i].b)
			{
				contain = true;
				break;
			}

			for (int j(0), size(tree.size()); j < size; j++)
			{
				if ((tree[j].a == current || tree[j].b == current) && !(tree[j] == Ribs[i]))
				{
					if (tree[j].a == current && !color[tree[j].b])
					{
						BFS.push(tree[j].b);
						color[tree[j].b] = 1;
					}
					else if (tree[j].b == current && !color[tree[j].a])
					{
						BFS.push(tree[j].a);
						color[tree[j].a] = 1;
					}
				}
			}
		}

		if (!contain)
			tree.push_back(Ribs[i]);
	}

	return tree;
}
vector<edge> graph::spintreeKraskalv2()
{
	//алгоритм краскала
	const int start = 0;
	vector<edge> tree;
	priority_queue<edge, std::vector<edge>, greater<edge>> T;

	for (int i(0); i < this->vertexNum; i++)
	{
		for (int j(0); j < G[i].size(); j++)
		{
			T.push(edge(i, G[i][j], this->lenth[i][j]));
		}
	}

	edge e;
	vector <int> intree; // Множество вершин 
	intree.resize(vertexNum);

	for (int i(0); i < vertexNum; i++)
		intree[i] = i;

	// Проверяем вершины каждого ребра. 
	// Если вершины не принадлежат одному и тому же поддереву, то такое ребро добавляем в наше дерево, а вершины помещаем в одно поддерево 
	while (!T.empty())
	{
		edge d = T.top();
		T.pop();
		if (intree[d.a] != intree[d.b])
		{
			tree.push_back(d);
			int new_intree = intree[d.a], old_intree = intree[d.b];
			for (int j = 0; j < vertexNum; j++)
				if (intree[j] == old_intree)
					intree[j] = new_intree;
		}
	}
	return tree;
}

void oriented_graph::read(std::fstream &file)
{
	if (!file.is_open())
		throw "graph::read::file not opened\n";

	file >> vertexNum >> ribNum;

	G.resize(vertexNum);
	lenth.resize(vertexNum);

	float weight;
	for (int i(0), ver1, ver2; i < ribNum; i++)
	{

		file >> ver1 >> ver2 >> weight;
		if (ver1 > G.size() || ver2 > G.size())
			throw "graph::read::graph overflow\n";

		G[ver1 - 1].push_back(ver2 - 1);

		lenth[ver1 - 1].push_back(weight);
	}
}

vector<edge> oriented_graph::transitive_closure()
{
	vector<edge> transitive_closure;

	for (int i(0); i < vertexNum; i++)
	{
		for (int j(0); j < vertexNum; j++)
		{
			bool exits = false;
			for (int k(0); k < G[i].size(); k++)
			{
				if (G[i][k] == j)
				{
					exits = true;
					break;
				}
			}

			if (exits)//if exits i -> j rib
			{
				for (int k(0); k < vertexNum; k++)
				{
					bool exits2 = false;
					for (int m(0); m < G[j].size(); m++)
					{
						if (G[j][m] == k)
						{
							exits2 = true;
							break;
						}
					}
					
					if (exits2)//if exits j->k rib
					{
						transitive_closure.push_back(edge(i, k));
					}
				}
			}
		}
	}

	return transitive_closure;
}

vector<edge> oriented_graph::transitive_closure_in_width()
{
	vector<edge> transitive_closure;

	vector<int> color;
	color.resize(vertexNum);
	for (int i(0); i < vertexNum; ++i)
		color[i] = 0;

	for (int i(0); i < vertexNum; ++i)
	{
		queue<int> q;
		int current = i;
		while (q.size())
		{
			//TODO(loject): continue
		}
	}

	return transitive_closure;
}

vector<int> oriented_graph::topological_sorting()
{
	vector<int> topological_sorting;
	vector<int> color;
	color.resize(vertexNum);
	for (auto i : color) i = 0;

	return topological_sorting;
}