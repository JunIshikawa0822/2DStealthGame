struct OctreeNode
{
    float3 center; // Center of the node
    float size;    // Size of the node (half the width)
    int objectIndex; // Index of the object if it exists
    int children[8]; // Indices of child nodes
};
