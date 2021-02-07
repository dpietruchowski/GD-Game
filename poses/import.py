import blendfile
from collections import deque

#handle = blendfile2.openBlendFile("model.blend")
#bfile = blendfile2.BlendFile(handle)
bfile = blendfile.open_blend("model.blend")

def print_fields(field, n = 0):
    if b'next' in field.dna_name.name_only:
        return
    if b'prev' in field.dna_name.name_only:
        return
    if b'lib' in field.dna_name.name_only:
        return
    if b'id' in field.dna_name.name_only:
        return
    spaces = " "*n
    #if b'MyPoseLib' in field.dna_name.name_only:
    #    print (f'{field.dna_name}')
    print(f'{spaces}{field.dna_name}')
    for f in field.dna_type.fields:
        print_fields(f, n+1)

def find_field(parent, field):
    if b'next' in parent.dna_name.name_only:
        return
    if b'prev' in parent.dna_name.name_only:
        return
    if b'newid' in parent.dna_name.name_only:
        return
    print(parent.dna_name)
    for f in parent.dna_type.fields:
        if field in f.dna_name.name_only:
            print('found')
        find_field(f, field)

def print_structs():
    for struct in bfile.structs:
        for field in struct.fields:
            if b'poselib' in field.dna_name.name_only:
                print_fields(field)
def loop():
    for struct in bfile.structs:
        structs = deque()
        for field in struct.fields:
            structs.append(field)
        while(len(structs) > 0):
            for f in field.dna_type.fields:
                structs.append(f)
            print(f.dna_name.name_only)


field_map = {}
def recursive(field):
    if field.dna_name.name_full in field_map:
        return
    field_map[field.dna_name.name_full] = field
    for f in field.dna_type.fields:
        recursive(f)

for struct in bfile.structs:
    for field in struct.fields:
        recursive(field)
    for name, field in field_map.items():
        if b'T-Pose' in name:
            print(name)
