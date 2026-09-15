BEECH_MATURE_01
European beech (Fagus sylvatica), mature woodland-edge modelling prototype.

Included: Unity-compatible FBX, editable Blender 4.0 source, bark albedo texture,
FBX texture folder, preview, AI-generated modelling reference and its prompt.

Approximately 25 metres tall. Ground-level pivot; Y-up FBX; two meshes.
355,022 triangles: 181,022 bark/branches and 174,000 leaves.
This is a high-detail prototype, not yet optimized for dense forest placement.
No LODs, collision, wind, chopping or other gameplay components are included.
The branching is procedural and the leaves use simple shaded geometry; this
is an interpretation of the reference, not a scan or photoreal finished asset.

Manual Unity import: copy the FBX and its .fbm texture folder together.
Use a shader appropriate to your rendering pipeline; for URP use URP/Lit.
Assign Beech_Bark_Albedo.png to the bark Base Map, with low smoothness.
The four leaf materials use green base colours. All materials are opaque.
Leaves contain front and back faces, so alpha clipping is not required.
The Blender file includes preview camera and lights; the FBX contains only the tree meshes.

Validation: FBX reimported into a clean Blender session; two meshes, UVs,
material assignments, texture paths and approximately 25 m height checked.
Preview rendered in Blender. Unity was neither opened nor modified and
Unity appearance/performance have not been tested.
