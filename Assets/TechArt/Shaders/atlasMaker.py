columnsLow = int(input("Insert min number of sprites in a row: "))
columnsHigh = int(input("Insert max number of sprites in a row: "))
resX = int(input("Insert width of each element in pixels: "))
resY = int(input("Insert height of each element in pixels: "))

from PIL import Image

def createImg(colNum):
    # create a new image with width 512 and height 512
    img = Image.new('RGB', (resX * colNum, resY), (255, 255, 255))
    for i in range(colNum):
        for x in range(0, resX):
            for y in range(0, resY):
                u = x / resX
                v = y / resY
                img.putpixel((x + resX * i, y), (int(u * 255), int(v * 255), 0))

    img.save("atlas/" + str(resX) + "x" + str(resY) + "-" + str(colNum) + ".png")

for i in range(columnsLow, columnsHigh + 1):
    createImg(i)