#pragma once

#include <iostream>
#include <vector>
#include <fstream>
#include <algorithm>	
#include <stack>
#include <queue>	
#include <map>

using namespace std;

struct edge
{
	int a, b;
	float lenth;
	edge() :a(0), b(0), lenth(-1) 
	{}
	edge(int a, int b, float lenth = -1) : a(a), b(b), lenth(lenth)
	{}

	friend bool operator>(const edge& left, const edge& right) { return bool(left.lenth > right.lenth); }
	friend bool operator<(const edge& left, const edge& right) { return bool(left.lenth < right.lenth); }
	friend bool operator==(const edge& left, const edge& right) { return 
		bool(((left.a == right.a && left.b == right.b) || 
		(left.a == right.b && left.b == right.a)) &&
		left.lenth == right.lenth); }
};

class graph
{
protected:
	int vertexNum, ribNum;
	vector<vector<int>> G;
	vector<vector<float>> lenth;

public:
	virtual void read(fstream& file);
	void out();

	vector<edge> spintree();
	vector<edge> spintree_v1();
	vector<edge> spintreeKraskal();
	vector<edge> spintreeKraskalv2();

	graph();
	~graph();
};

class oriented_graph : private graph
{
public :
	void read(fstream& file);
	vector<edge> transitive_closure();
	vector<edge> transitive_closure_in_width();

	vector<int> topological_sorting();
};
