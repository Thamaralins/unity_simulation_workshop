import os
import glob
import json
import shutil
import argparse
from tqdm import tqdm

def get_arguments():
    parser = argparse.ArgumentParser(description="Solo to LabelMe converter.")
    
    parser.add_argument("solo_path", help="Path to solo dataset")
    parser.add_argument("output_path", help="Path to output directory")

    return parser.parse_args()
    
def create_labelme_dict():
    return {
        "version": "3.16.7",
        "flags": {},
        "shapes": [],
        "imagePath": None,
        "imageData": None,
        "imageHeight": None,
        "imageWidth": None
    }

def create_shape_dict():
    return {
        "label": None,
        "points": [
            [None, None],
            [None, None]
        ],
        "group_id": None,
        "description": "",
        "shape_type": "rectangle",
        "flags": {}
    }

if __name__ == '__main__':
    args = get_arguments()

    # Create output directory
    if not os.path.exists(args.output_path):
        os.makedirs(args.output_path)
    
    dataset_path = os.path.join(args.solo_path, "sequence.0")
    json_files = glob.glob(os.path.join(dataset_path, "*.json"))

    i = 0

    for json_file in tqdm(json_files, desc=f"Progress", ncols=100):
        print(i)
        i += 1

        json_dict = json.load(open(json_file, "r"))
        labelme_dict = create_labelme_dict()
        
        image_filename = json_dict["captures"][0]["filename"]
        image_size = json_dict["captures"][0]["dimension"]
        json_annotations = json_dict["captures"][0]["annotations"][0]

        image_path = os.path.join(dataset_path, image_filename)
        output_image_path = os.path.join(args.output_path, image_filename)
        shutil.copy(image_path, output_image_path)

        labelme_dict["imagePath"] = image_filename
        labelme_dict["imageHeight"] = image_size[0]
        labelme_dict["imageWidth"] = image_size[1]

        if "values" in json_annotations.keys():
            labelme_dict["shapes"] = list()
            for annotation in json_annotations["values"]:
                label_name = annotation["labelName"]
                origin = annotation["origin"]
                dimension = annotation["dimension"]

                point_1 = list()
                point_2 = list()

                point_1.append(origin[0])
                point_1.append(origin[1])
                point_2.append(origin[0] + dimension[0])
                point_2.append(origin[1] + dimension[1])

                shape_dict = create_shape_dict()
                shape_dict["label"] = label_name
                shape_dict["points"][0] = point_1
                shape_dict["points"][1] = point_2

                labelme_dict["shapes"].append(shape_dict)
                
                print(label_name)
                print(origin)
                print(dimension)
        print()

        json_object = json.dumps(labelme_dict, indent=2)

        output_json_file = output_image_path.replace(".png", ".json")
        with open(output_json_file, "w") as outfile:
            outfile.write(json_object)

