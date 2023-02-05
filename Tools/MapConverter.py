#！/usr/bin/env python3

from string import Template

resource_list = [
    {
        "path": "res://Scenes/Player.tscn",
        "type": "PackedScene",
        "id": 1,
        "char": "P",
        "count": 0
    },
    {
        "path": "res://Scenes/Earth/Earth.tscn",
        "type": "PackedScene",
        "id": 2,
        "char": "E",
        "count": 0
    },
    {
        "path": "res://Scenes/Earth/DryEarth.tscn",
        "type": "PackedScene",
        "id": 3,
        "char": "T",
        "count": 0
    }
]

input_filename = input("Input file path: ")
width = int(input("Width: "))
output_filename = input("Output file path: ")

with open(output_filename, "w") as output_file:
    ext_res = ""
    for resource in resource_list:
        ext_res += f"[ext_resource path=\"{resource['path']}\" type=\"{resource['type']}\" id={resource['id']}]\n"

    player_position_x = 0
    player_position_y = 0

    with open(input_filename, "r") as input_file:
        result = ""
        x = 0
        y = 0
        for line in input_file:
            for c in line:
                if c == 'A':
                    x += width
                    continue
                if c == 'P':
                    player_position_x = x
                    player_position_y = y
                    x += width
                    continue
                for resource in resource_list:
                    if c == resource['char']:
                        result += f"\n[node name=\"{c + str(resource['count'])}\" parent=\"Map\" instance=ExtResource( {resource['id']} )]\n"
                        result += f"position = Vector2( {x}, {y} )\n"
                        resource['count'] += 1
                x += width
            y += width
            x = 0

    with open("./template.tscn", "r") as template_file:
        template = Template(template_file.read())
        output_file.write(template.substitute(ext_resource=ext_res, PlayerPositionX=player_position_x, PlayerPositionY=player_position_y, nodes=result))

