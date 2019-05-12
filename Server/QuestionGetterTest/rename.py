import os, re

""" rename pictures
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
"""
def repl(matched):
    num = str(matched.group('num'))
    name = str(matched.group('name'))
    return '\\\\'+num+name

reg = r'\\\\(?P<name>(.+?).png)(?P<num>\d+)'
for i in [0,1,2,5,6,7]:
	file = 'subject_'+str(i)+'.que'
	with open(file, 'r', encoding = 'utf-8') as f1:
		data = f1.read()
		data = re.sub(reg, repl, data)
		
		with open(file+'.bak', 'w', encoding = 'utf-8') as f2:
			f2.write(data)
