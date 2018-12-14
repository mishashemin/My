#include "graph.h"

using namespace std;
graph::graph():vertexNum(0), ribNum(0) {}


graph::~graph() {}

void graph::read(fstream& file)
{

	if(!file.is_open())
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
	for (int i(0); i < ribNum; i++)
	{
		cout << _graph[i].a -1<< " " << _graph[i].b-1 << _graph[i].lenth << endl;
    }
}

void graph::spintree_out()
{
	if (spintree_graph.size() == 0) spintree_graph = spintree_Prim();
	
	for (int i(0); i < spintree_graph.size(); i++)
	{
		cout << spintree_graph[i].a << " " << spintree_graph[i].b << spintree_graph[i].lenth << endl;
	}
}

vector<edge> graph::spintree_Prim()
{	
	const int start = 0;
	vector<edge> tree;

	vector<int> intree;
	intree.resize(vertexNum);
	for (int i(0); i < vertexNum; i++)
		intree[i] = 0;

	priority_queue<edge, vector<edge>, greater<edge>> T;
	
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
vector<edge> graph::spintree_Kraskal()
{
	//алгоритм краскала
	const int start = 0;
	vector<edge> tree;
	priority_queue<edge, vector<edge>, greater<edge>> T;

	for (int i(0); i < this->vertexNum; i++)
	{
		for (int j(0); j < G[i].size(); j++)
		{
			T.push(edge(i, G[i][j], this->lenth[i][j]));
		}
	}

	edge e;
	vector <int> intree; 
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


