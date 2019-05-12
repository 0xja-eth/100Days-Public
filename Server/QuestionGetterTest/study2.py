# coding=utf-8    
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import re
import time
import os

print ("start")
#打开Chrome浏览器
driver = webdriver.Chrome("E:/InstallPackages/chromedriver_win32/chromedriver.exe")
"""
cookies = {
	'jyean': 'vRH-FZrJWihnMI1yHn_Fef-ULaF7cKW4h6m0EakX0wiBw5iG9h1ynRX2W0jTF7J0-XDq0CBekd8pBdqkqAdy0JwaAP7IKz0-wlT5MOhmtksIaRxyeg1dtTmoa87_z3Df0',
	'jye_cur_sub':'chinese2',
	'UM_distinctid':'16a8ace81b716-03b9d681155e01-b79183d-144000-16a8ace81b94c4',
	'CNZZDATA2018550':'cnzz_eid%3D1991036963-1557103624-%26ntime%3D1557103624',
	'jy_user_lbs_chinese2': '',
	'jy' : '30967C01F1EAAB945E89C75A10A502CBDA0612B3D92260B7C982FC8D53B7909F6EFE71F07367ED72D217D6D27753EA8B972186FF4D268FBC6C3AF3CCB2E3EB9525E22B66D3C52B23565C96722B3EEE0FF2E80951552DCAD7819282CC3EDE2E65BBC26E714F3A3F0CBD881AC263C03A36AE2E05CF0B2F2E4DF6BF490B4C41EC600D8003CD0F472D6DA37BB224FA1AF822306BCEEBC56883F3AA28E0A099354196CB7A0816AFDB3055E1FD4FD2061391399015B3A7EDBE6A3407F06D302A0C6108A431942BCDEA63B0453B3943F9FAB6E0133EE68AD30CAC640E6633ECB61FF1E08FE598A795BDFB8BB977CD67909E05F875E9F0522301D3B87F971B681F434FEF893ACA9064F5BFEB7E6D39490FF8D9E4DE3BA2FAC10C1A17065656130262068F3AF41F023C706A7E78067243E91E9D13C3F04342871F20D5D81EB9CA9A9634F2B3F4A5ECB5C22825B5DFADFA4F947979',
	'jye_notice_yd_notenough' : '1|2019/5/6 22:42:52|0|false'
}
"""
cookies = {
	'jyean': 'p2_SyR-E5vs7Cx5lVFqxSmOmZVRcLovx9qta4Q_0yVaaulz6IDIOmVqcE6AHzFLKjYm209kHlyQ_xip1ORqi8s2Q6z5eMP19xaIAm8H83dtF4QCUuIYNXEPPG80Ied8d0',
	'jye_cur_sub':'chinese2',
	'UM_distinctid':'16a71fb093252-0c3f9bcb638a84-b79183d-144000-16a71fb09333e',
	'CNZZDATA2018550':'cnzz_eid%3D1027833966-1556689497-%26ntime%3D1557165618',
	'jy_user_lbs_chinese2': '',
	'jy_user_lbs_math2': '',
	'jy' : '117CE7EE40245E9600E462E6B2416AC07C0D68A010748F3A074EF84821B834A0843D6C034D7579D3806DDA63AD38C09200BF85F0355436CB8DD404D2AD6541790A69FA9AA55EE1AA9C2BF7091686AADEEF4482BA6D24178B4187E84076943805BFC51BB6DF8F946F068F15B0CA7678A04DFA49B8CBEB884ACF264ECE4B72D40388BEEDD6ABEC77983DBD9BD3A7429FF7C7C118B419D12EB273828D3C4AEDB7C9C389F1A910574E0D7BB951916492C335EB8CF182943971B995A5150A534CFAC6FE77CD9DBE2CE8A72B15013F891421B4E01C7510101F685F7200E0A633675AA64AAE778AC44B400C251871F2591347333DBE32519EECEC676E1B1F7F1FC6E0288055C2B0958C1DEDA67338BDF885DF0FFF8C2ACECEEFB9230EAEDDB8D30AF5FB0E01A202C378930CD7EB2A23591E8681C7AA900C175038850FD0320C899A79528475D939E744D9D2F5E68162BEAAB454',
	'__RequestVerificationToken' : 'R-8l1irB9d-fWAYTKPaSluftncu-jy0jsEE-EENZrIfJjNMebZwzTg-IrCKuzF4pPBmPPvp_qN1W_RU0jyBp91oeIXVFUzYaZ73FIiCy2Dc1'
#	'jye_notice_yd_notenough' : '1|2019/5/7 02:42:52|0|false'
}

#定位节点
base_url = "http://www.jyeoo.com/chinese2/ques/search?f=0&q=1dc14bc4-ddf8-4ea9-9c7a-b767c29c3043~01525d06-ed64-4403-ac49-1b8c93b38f2a~&lbs=&ct=1&pd=1"
#url = 'http://www.jyeoo.com/chinese2/ques/detail/e5008d45-2630-47e0-ad09-705e671c4d4a'
print (base_url)
driver.get(base_url)

for key in cookies:
	driver.add_cookie({'name':key, 'value':cookies[key]})
time.sleep(3)
driver.get(base_url)

"""
time.sleep(5)
print (5)
time.sleep(5)
print (10)
time.sleep(5)
print (15)
time.sleep(5)
print (20)
time.sleep(5)
"""
time.sleep(2)
content = driver.find_elements_by_xpath("//div[@class='mid-content']")
print(content)
cnt = 0
for c in content:
	ques = c.find_elements_by_class_name("QUES_LI")
	for q in ques:
		cnt += 1
		#print(str(cnt)+":\n"+q.text)
		bottom = q.find_elements_by_class_name("fieldtip")[0]
		btn = bottom.find_element_by_tag_name("a")

		#for btn in btns:
		print(btn.text)
		btn.click()
		ctx = WebDriverWait(driver, 10).until(
	        EC.presence_of_element_located((By.CLASS_NAME, "body-content"))
	    )
		time.sleep(2)
		#ctx = driver.find_elements_by_xpath("//div[@class='body-content']")[0]
		close = driver.find_elements_by_xpath("//input[@class='smbtn hclose']")[0]

		try:
			q2 = ctx.find_element_by_class_name("QUES_LI")

			print(ctx.text)

			pt1 = q2.find_element_by_class_name("pt1")

			print ("Window Position: "+str(driver.get_window_position()))
			print ("Location = "+str(pt1.location))

			data = pt1.screenshot_as_png

			driver.save_screenshot("screenshot.png")

			with open('screenshot'+str(cnt)+'.png', 'wb') as file:
				file.write(data)

			pt2 = q2.find_element_by_class_name("pt2")
			choices = []
			answers = []
			chos = pt2.find_element_by_tag_name("table").find_element_by_tag_name("tbody").find_elements_by_tag_name("td")
			for cho in chos:
				choices.append(cho.text)
				ans = cho.find_elements_by_xpath(".//input[@checked='checked']")
				answers.append(len(ans) > 0)


			pt5 = q2.find_element_by_class_name("pt5")
			pt6 = q2.find_element_by_class_name("pt6")
			pt7 = q2.find_element_by_class_name("pt7")


			with open('question'+str(cnt)+'.que', 'w', encoding='utf-8') as file:
				file.write('TIT:'+pt1.text)
				file.write('\nCHO:\n')
				for choice in choices:
					file.write(choice+'\n')
				file.write('\nANS:')
				for i in range(len(answers)):
					if answers[i]:
						file.write(chr(ord('A')+i))
				file.write('\nDES:\n')
				file.write(pt5.text+'\n')
				file.write(pt6.text+'\n')
				file.write(pt7.text+'\n')
			print("success")
		except:
			print("fail")

		close.click()
		time.sleep(0.5)

