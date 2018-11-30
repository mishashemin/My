#include <fstream>
#include <iostream>

#include "graph.h"

using namespace std;

int main()
{
	graph G;
	fstream file("input.txt", ios_base::in);

	cout << "read file" << endl;
	try {
		G.read(file);
	}
	catch (const char *error)
	{
		cout << error;
		system("pause");
		return -1;
	}

	cout << "file readed succesful" << endl;
	cout << "\n";

	vector<edge> tree = G.spintree();
	for (int i(0), size(tree.size()); i < size; i++)
	{
		cout << tree[i].a + 1 << " - " << tree[i].b + 1 << " - " << tree[i].lenth << ";\n";
	}

	system("pause");
	return 0;
}