import os, re
for _, _, files in os.walk('pictures'):
	for file in files:
		name = os.path.splitext(file)[0]
		ext = os.path.splitext(file)[1]
		if ext != '.png':
			
			num = int(ext[4:])
			new_name = str(num)+name+'.png'

			print("file: "+file+"  new: "+new_name)
			os.system('COPY pictures\\'+file+' pictures\\'+new_name)#+' '+new_name)
			os.system('DEL pictures\\'+file)#+' '+new_name)
