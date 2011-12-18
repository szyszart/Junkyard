# Sprite sheet metadata extractor.
# Written by Krzysztof Kusiak.
# If you find it useful, use it all the way you like. :-)
import sys

def filenames(lines):
    result = []
    for i in range(0, len(lines), 2):
        line = lines[i]
        name = line.split()[0].split('/')[-1]        
        result.append(name)
    return result

def process_convert_log(lines):    
    geometry = []
    for i in range(1, len(lines), 2):
        line = lines[i]        
        words = line.split()
        dims = words[2].split('=>')[1]
        offsets = words[3].split('+')
        x, y = int(offsets[1]), int(offsets[2])
        width, height = map(int, dims.split('x'))
        geometry.append((x, y, width, height))
    return zip(filenames(lines), geometry)        

def output_geometry(geometry, tiles_per_row):
    count = 0
    x, y = 0, 0
    maxh = 0
    print 'frames = {'
    for item in geometry:
        offx, offy, w, h = item[1]        
        maxh = max(maxh, h)
        print '\t{ x = %s, y = %s, offsetx = %s, offsety = %s, width = %s, height = %s },' % (x, y, offx, offy, w, h)
        count += 1  
        x += w
        if count % tiles_per_row == 0:
            x = 0
            y += maxh
            maxh = 0
    print '}'
                            
def show_usage(program_name):
    print 'Usage: %s convert_log [tiles_per_row]' % program_name    
    
DEFAULT_TILES_PER_ROW = 5    
    
if __name__ == '__main__':
    if len(sys.argv) < 2 or len(sys.argv) > 3:
        show_usage(sys.argv[0])
        sys.exit(1)
        
    try:
        tiles_per_row = int(sys.argv[2])
    except:
        print 'Invalid tiles per row argument. Defaulting to %s.' % DEFAULT_TILES_PER_ROW
        tiles_per_row = DEFAULT_TILES_PER_ROW
        
    try:
        with open(sys.argv[1]) as log:
            contents = log.readlines()            
            geometry = process_convert_log(contents)
            output_geometry(geometry, tiles_per_row)
    except EnvironmentError:
        print 'Cannot open %s.' % sys.argv[1]
        sys.exit(2)