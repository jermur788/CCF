# Sessile oak — three development stages

Procedural summer-foliage modelling prototypes for **Quercus petraea**.

- Sapling: approximately 2 m tall, slender leader and light branching.
- Young tree: approximately 8 m tall, developing upright crown.
- Mature tree: approximately 20 m tall, stronger trunk, spreading scaffold branches and a broad irregular crown.

Each stage includes an editable Blender 4.0 source, a GLB at highest detail,
three separate FBX detail levels, and a rendered preview. Names end in LOD0
(highest), LOD1, and LOD2 (lowest). These are detail alternatives for the same
stage, not additional ages. Asset_Info.json gives triangle counts;
Validation.json records independent FBX reimport checks.

Leaves have rounded lobes, tapered bases and visible petioles. They are solid
geometry with separated front/back surfaces and four green colour variations;
no alpha-cutout shader is needed. Leaf size is exaggerated in the larger stages
for a readable game silhouette. Bark has a portable embedded albedo texture.
These are artistic approximations rather than botanical scans. Acorns, seasonal
variants, wind weights, collision, chopping and simulation logic are not included.

Botanical reference: Forest Research, Sessile oak:
https://www.forestresearch.gov.uk/tools-and-resources/tree-species-database/131560-sessile-oak-sok/
Also Woodland Trust Nature's Calendar, sessile oak leaf identification:
https://naturescalendar.woodlandtrust.org.uk/what-we-record-and-why/species-we-record/trees/oak-sessile/

## Unity import

FBX exports use Y-up, -Z forward, metre units and ground-level trunk-centre pivots.
Each FBX contains bark/branches and foliage as two meshes. Import the FBX and
assign opaque URP/Lit materials if Unity's material conversion does not choose
your render pipeline. Use Oak_Bark_Albedo.png for bark, low smoothness for all
materials, and the four leaf colours supplied by the source materials.

Use one detail level per tree, or configure a Unity LODGroup with the three FBXs.
Do not display all three levels simultaneously. The highest-detail meshes are high-poly prototypes (about 41k / 273k / 607k
triangles for sapling / young / mature). Lowest detail is about 2.8k / 18k / 40k.
Use the detailed meshes for close inspection; dense forests may need additional
optimization, foliage cards or impostors. Profile placement in your scene.
No automatic LODGroup or Unity prefab is supplied. Unity rendering and runtime
performance have not been tested; validation reimports the FBXs into Blender.

To scale to a chosen visual height H, use uniform scale H / authored height.
Model dimensions and developmental labels do not define simulation age, DBH,
crown size, species registration, or regeneration behaviour.

## Editable sources and reproducibility

The .blend files contain all three detail levels of that stage, LOD0 visible,
plus preview camera, lights and ground. Preview objects are not exported to FBX
or GLB. The bark image is packed into the sources and embedded into FBX/GLB.
Build and validation scripts are included. Run them only in a background or
fresh Blender session; they clear the active scene. Blender's FBX/GLB exporters
require NumPy. The scripts optionally look in /tmp/oak-blender-libs, used during
creation; an ordinary Blender installation with NumPy needs no such folder.
