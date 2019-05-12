from bs4 import BeautifulSoup
import requests
url = "http://www.jyeoo.com/chinese2/ques/detail/e5008d45-2630-47e0-ad09-705e671c4d4a"
#发送请求
content = requests.get(url,params=None).content.decode("utf-8")

html = BeautifulSoup(content,"html.parser")
print("type(html) = "+str(type(html)))
#查找每个练习的a标签的href属性
content=html.find(class_="quesborder")
print("content = "+str(content))
title=content.find(class_="pt1")
print("title = "+str(title))

answer=content.find(class_="pt2").table.find_all("label")
print("answer = "+str(answer))
desc=content.find(class_="pt6")
print("desc = "+str(desc))
#创建一个列表保存url
"""
url_list=[]
for x in a:
    url_list.append("http://www.runoob.com"+x["href"])
    print(x)


datas=[]
for i in range(2):
    print("for "+str(i)+":")
    dic = {}
    html01 = requests.get(url_list[i]).content.decode("utf-8")
    soup02 = BeautifulSoup(html01, "html.parser")
    print("content = "+str(soup02.find(class="article-intro")))
    dic['title'] = soup02.find(id="content").h1.text
    # 题目
    dic['content01'] = soup02.find(id="content").p.next_sibling.next_sibling.text
    # print(content01)
    # 程序分析
    dic['content02'] = soup02.find(id="content").p.next_sibling.next_sibling.next_sibling.next_sibling.text
    try:
        dic['content03'] = soup02.find(class_="hl-main").text
    except Exception as e:
        dic["content03"] = soup02.find("pre").text
    print("dic = "+str(dic))
    with open("100_py.csv","a+",encoding="utf-8") as file:
        file.write(dic['title']+"\n")
        file.write(dic['content01']+"\n")
        file.write(dic['content02']+"\n")
        file.write(dic['content03']+"\n")
        file.write("*"*60+"\n")
"""