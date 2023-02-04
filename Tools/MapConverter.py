from string import Template

resource_list = [
    {
        "path": "res://Scenes/Player.tscn",
        "type": "PackedScene",
        "id": 1,
        "char": "P",
        "count": 0
    }
]

input_filename = input("Input file path: ")
player_position_x, player_position_y = input("Player position (x, y): ").split(", ")
width = int(input("Width: "))
output_filename = input("Output file path: ")

with open(output_filename, "w") as output_file:
    ext_res = ""
    for resource in resource_list:
        ext_res += f"[ext_resource path=\"{resource['path']}\" type=\"{resource['type']}\" id={resource['id']}]\n"

    with open("template.tscn", "r") as template_file:
        template = Template(template_file.read())
        output_file.write(template.substitute(ext_resource=ext_res, PlayerPositionX=player_position_x, PlayerPositionY=player_position_y))

    with open(input_filename, "r") as input_file:
        x = 0
        y = 0
        for line in input_file:
            for c in line:
                for resource in resource_list:
                    if c == resource['char']:
                        output_file.write(f"\n[node name=\"{resource['count']}\" parent=\"Map\" instance=ExtResource( \"{resource['id']}\" )]\n")
                        output_file.write(f"position = Vector2( {x}, {y} )\n")
                        resource['count'] += 1
                x += width
            y += width
            x = 0
