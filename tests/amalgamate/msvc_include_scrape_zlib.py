# LICENSE
#
#   This software is dual-licensed to the public domain and under the following
#   license: you are granted a perpetual, irrevocable license to copy, modify,
#   publish, and distribute this file as you see fit.


import os
import re

def fts_msvc_include_scrape(config):

    source_pattern = r'\d>\s*(.*\.c(pp)?)$'
    include_pattern = r'including file: (\s*)(.*)$'
    ignore_pattern = r'program files'

    # Open build output
    print "Opening output file: {0}".format(config['msvc_build_output'])
    build_output = open(config['msvc_build_output'], 'r')

    # Track which files have already been included
    source_files = set()
    include_files = []
    include_files_set = set() # Unordered for fast lookup

    # Loop over build output line by line
    for line in iter(build_output):

        # Source file
        match = re.search(source_pattern, line)
        if match:
            source_file = match.group(1)
            source_files.add(source_file)
            continue

        # Include file
        match = re.search(include_pattern, line)
        if match:
            level = len(match.group(1))
            include_file = match.group(2)

            # Continue if file has already been included
            if include_file in include_files_set:
                continue
            
            # Some files should be ignored
            match = re.search(ignore_pattern, line, re.IGNORECASE)
            if match:
                continue

            # Build relative path
            include_file = os.path.relpath(include_file, config['source_root'])

            # Skip repeats
            if include_file in include_files_set:
                continue
            
            # Store include file
            include_files_set.add(include_file)
            include_files.append([level, include_file])

    


    # Print source files with relative path
    print "\nSource Files:"
    for root, dirs, files in os.walk(config['source_root']):
        for name in files:
            if name in source_files:
                absolute_path = os.path.join(root, name)
                relative_path = os.path.relpath(absolute_path, config['source_root'])
                print relative_path

    # Print ordered include files
    print "\nInclude Files:"
    file_stack = []
    last_level = -1
    for level, include_file in include_files:
        while file_stack and file_stack[-1][0] >= level:
            stack_level, stack_file = file_stack.pop()
            print stack_file

        file_stack.append([level, include_file])
        last_level = level
    
    while file_stack:
        stack_level, stack_file = file_stack.pop()
        print stack_file


def main():
    config = {}
    config['msvc_build_output'] = "msvc_build_output_zlib.txt"
    config['source_root'] = 'c:\\temp\zlib\\'
    fts_msvc_include_scrape(config)


if __name__ == "__main__":
    main()