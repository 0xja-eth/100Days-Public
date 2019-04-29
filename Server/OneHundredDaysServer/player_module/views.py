from django.shortcuts import render
from django.conf import settings
from utils.view_func_utils import processRequest, getErrorResponse, getSuccessResponse, convertRequestDataType
from utils.exception import ErrorType, ErrorException
from player_module.models import Player, School

def player_create(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['name', 'school'])

		name = data['name']
		school = data['school']

		createPlayer(name, school)

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	return getSuccessResponse()

def player_save(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['name'], FILES=['save'])

		convertRequestDataType(data, ['save'], 'save')

		name = data['name']
		save = data['save']

		pushSavefile(name, save)

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	return getSuccessResponse()

def school_get(request):
	try:
		# 获取数据
		data = processRequest(request)

		result = getSchools('dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': result}
	return getSuccessResponse(dict)

def createPlayer(name, school):
	"""
	创建玩家

	:param name:  玩家名字
	:param school:  学校名字
	"""
	ensurePlayerNotExists(name)

	school_obj = None

	if not isSchoolExists(school):
		school_obj = createSchool(school)
	else:
		school_obj = School.objects.get(name=school)
		
	player = Player()
	player.name = name
	player.school = school_obj
	player.save()

	return player

def createSchool(name):
	"""
	创建学校

	:param name:  学校名字
	"""
	if not isSchoolExists(name):
		school_obj = School()
		school_obj.name = name
		school_obj.save()

		return school_obj
	return None

def pushSavefile(name, save):
	"""
	上传存档文件

	:param name:  玩家名字
	:param save:  文件路径
	"""
	ensurePlayerExists(name)

	player = Player.objects.get(name=name)
	player.savefile = save
	player.save()

	player.refresh()

def getSchools(return_type='QuerySet'):
	"""
	获取全部学校

	:return:  学校(QuerySet)
	"""
	result = School.objects.all()

	if return_type == 'dict':
		temp = []
		for r in result:
			temp.append(r.name)
		result = temp

	return result 


def isPlayerExists(name):
	"""
	玩家是否存在

	:param name:  玩家名字
	"""
	return Player.objects.filter(name=name).exists()

def isSchoolExists(name):
	"""
	学校是否存在

	:param name:  学校名字
	"""
	return School.objects.filter(name=name).exists()


def ensurePlayerExists(name):
	"""
	保证玩家存在，不存在时抛出异常
	:param name:  玩家名字
	"""
	if not isPlayerExists(name):
		raise ErrorException(ErrorType.PlayerNotExist)

def ensurePlayerNotExists(name):
	"""
	保证玩家不存在，存在时抛出异常
	:param name:  玩家名字
	"""
	if isPlayerExists(name):
		raise ErrorException(ErrorType.PlayerExist)

def ensureSchoolExists(name):
	"""
	保证玩家存在，不存在时抛出异常
	:param name:  学校名字
	"""
	if not isSchoolExists(name):
		raise ErrorException(ErrorType.SchoolNotExist)
