#include <fstream>
#include <iostream>

#include "graph.h"

using namespace std;

int main()
{
	graph G;
	fstream file("input.txt", ios_base::in);
	
	try {
		G.read(file);
	}
	catch (const char *error)
	{
		cout << error;
		system("pause");
		return -1;
	}
	cout << "graph:\n";
	G.out();
	cout << endl;
	cout << "spintree_graph:\n" << endl;
	G.spintree_out();
	cout << "\n";
	system("pause");
	return 0;
}