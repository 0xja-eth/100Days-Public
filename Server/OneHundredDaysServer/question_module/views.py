import random
from itertools import chain
from django.shortcuts import render
from django.core.exceptions import DoesNotExist
from django.conf import settings
from utils.view_func_utils import processRequest, getErrorResponse, getSuccessResponse
from utils.exception import ErrorType, ErrorException
from question_module.models import Question
from player_module.models import Player

class QuestionDistributionType(Enum):
	Normal			=   0
	OccurFirst		=   1
	NotOccurFirst	=   2
	WorngFirst		=   3
	CorrFirst		=   4
	SimpleFirst		=   5
	MiddleFirst		=   6
	DifficultFirst	=   7

# Create your views here.
def query_id(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['id'])

		id = data['id']

		data = query('id', id, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_all(request):
	try:
		# 获取数据
		data = processRequest(request)

		data = query('all', return_type='dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_filter(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['type', 'param'])

		type = data['type']
		param = data['param']

		data = query(type, param, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query(filter_type,  param=None, return_type='QuerySet'):
	"""
	查询题目

	:param filter_type:  过滤类型("all","id","subject","level","type")
	:param return_type:  返回类型("QuerySet","dict")
	:param param: 参数
	:return: 题目数组(Question[])
	"""
	result = []

	if filter_type == 'all':
		result = Question.objects.all()
	elif filter_type == 'id':
		result = Question.objects.filter(id=param)
	elif filter_type == 'subject':
		result = Question.objects.filter(subjectID=param)
	elif filter_type == 'level':
		result = Question.objects.filter(level=param)
	elif filter_type == 'type':
		result = Question.objects.filter(typeID=param)

	if return_type == 'dict':
		for i in range(len(result)):
			result[i] = result[i].convert_to_dict

	return result


def generate_all(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['sid', 'type', 'count', 'name'])

		sid = data['sid']
		type = data['type']
		name = data['name']

		data = query('id', id, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate(generate_type, sid, distribute_type, count, name, param=None):
	"""
	生成题目

	:param generate_type:  生成类型("all","type","level")
	:param sid: 科目代号
	:param distribute_type:  分配类型(Enum)
	:param count: 数量
	:param name: 玩家名称
	:param param: 附加参数
	:return: 题目数组(Question[])
	"""
	try:
		player = Player.get(name=name)
	except DoesNotExist:
		raise ErrorException(ErrorType.PlayerNotExist)

	player.getSavefileInfo()

	questions = []

	sub = result = query('subject', sid)

	value = player.getSubjectValue(sid)
	max_level = getMaxLevel(value)

	if distribute_type >= QuestionDistributionType.SimpleFirst:
		# 难度模式
		min_level = 0
		if distribute_type == QuestionDistributionType.SimpleFirst:
			max_level = int(max_level/2)
		elif distribute_type == QuestionDistributionType.MiddleFirst:
			min_level = int(max_level/4)
			max_level = int(max_level/4*3)
		elif distribute_type == QuestionDistributionType.DifficultFirst:
			min_level = int(max_level/2)

		sub = result.filter(level__in=range(min_level,max_level))

	elif distribute_type >= QuestionDistributionType.Normal:
		# id 限制
		id_limit = []
		exclude = False
		if distribute_type == QuestionDistributionType.OccurFirst:
			id_limit = player.getDoneRec()
		elif distribute_type == QuestionDistributionType.NotOccurFirst:
			id_limit = player.getDoneRec()
			exclude = True
		elif distribute_type == QuestionDistributionType.WorngFirst:
			id_limit = player.getWrongRec()
		elif distribute_type == QuestionDistributionType.CorrFirst:
			id_limit = player.getDoneRec()
		
		if exclude:
			sub = result.exclude(id__in=id_limit)
		else
			sub = result.filter(id__in=id_limit)
		
		if distribute_type == QuestionDistributionType.CorrFirst:
			sub = sub.exclude(id__in=player.getWrongRec())

	for i in range(count):
		if generate_type == 'all':

		elif generate_type == 'type':

		elif generate_type == 'level':

	return questions

def getMaxLevel(value):

	level = -1
	values = settings.GAME_SETTING['QUESTION']['EntryValue']
	for val in values:
		if value < val: 
			return level
		level += 1

	return level
