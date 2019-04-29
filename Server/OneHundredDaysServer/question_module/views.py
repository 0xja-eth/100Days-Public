import random
from enum import Enum
from itertools import chain
from django.shortcuts import render
from django.core.exceptions import ObjectDoesNotExist
from django.conf import settings
from player_module.views import ensurePlayerExists
from utils.view_func_utils import processRequest, getErrorResponse, getSuccessResponse, convertRequestDataType, convertRequestDataTypeAll
from utils.exception import ErrorType, ErrorException
from question_module.models import Question, QuestionType, Subject, Choice
from player_module.models import Player

class QuestionDistributionType:
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

		convertRequestDataTypeAll(data, 'int')

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

		convertRequestDataTypeAll(data, 'int')

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
		result = Question.objects.filter(subject_id=param)
	elif filter_type == 'level':
		result = Question.objects.filter(level=param)
	elif filter_type == 'type':
		result = Question.objects.filter(type_id=param)

	if return_type == 'dict':
		temp = []
		for r in result:
			temp.append(r.convertToDict())
		result = temp

	return result


def generate_all(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count'], 'int')

		sid = data['sid']
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']

		data = generate('all', sid, dtb_type, count, name, None, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate_type(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name', 'type'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count', 'type'], 'int')

		sid = data['sid']
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']
		type = data['type']

		data = generate('type', sid, dtb_type, count, name, type, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate_level(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name', 'level'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count', 'level'], 'int')

		sid = data['sid']
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']
		level = data['level']

		data = generate('level', sid, dtb_type, count, name, level, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate(generate_type, sid, distribute_type, 
	count, name, param=None, return_type='QuerySet'):
	"""
	生成题目

	:param generate_type:  生成类型("all","type","level")
	:param sid: 科目代号
	:param distribute_type:  分配类型(Enum)
	:param count: 数量
	:param name: 玩家名称
	:param param: 附加参数
	:param return_type:  返回类型("QuerySet","dict")

	:return: 题目数组(Question[])
	"""
	ensurePlayerExists(name)

	player = Player.objects.get(name=name)

	result = []

	questions = query('subject', sid)

	ignore_level = False

	if generate_type == 'type':
		type = QuestionType.objects.get(id=param)
		questions = questions.filter(type=type)
	elif generate_type == 'level':
		questions = questions.filter(level=param)
		ignore_level = True

	sub = questions

	if not ignore_level:
		value = player.getSubjectValue(sid)
		max_level = getMaxLevel(value)

	#distribute_type = QuestionDistributionType(distribute_type)

	if not ignore_level and distribute_type >= QuestionDistributionType.SimpleFirst:
		# 难度模式
		min_level = 0
		if distribute_type == QuestionDistributionType.SimpleFirst:
			max_level = int(max_level/2)
		elif distribute_type == QuestionDistributionType.MiddleFirst:
			min_level = int(max_level/4)
			max_level = int(max_level/4*3)
		elif distribute_type == QuestionDistributionType.DifficultFirst:
			min_level = int(max_level/2)

		sub = questions.filter(level__in=range(min_level,max_level))

	elif not ignore_level and distribute_type == QuestionDistributionType.Normal:
		sub = questions.filter(level__in=range(0,max_level))

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
			sub = questions.exclude(id__in=id_limit)
		else:
			sub = questions.filter(id__in=id_limit)
		
		if distribute_type == QuestionDistributionType.CorrFirst:
			sub = sub.exclude(id__in=player.getWrongRec())

	random.shuffle(sub)
	for i in range(count):

		question = None
		if i < len(sub):
			question = sub[i]
		else:
			ltd = settings.GAME_SETTING['QUESTION']['GenerateTimesLimit']
			index = random.randint(0,len(questions)-1)
			question = questions[index]
			while ltd > 0 and question in result:
				index = random.randint(0,len(questions)-1)
				question = questions[index]
				ltd -= 1

		if return_type == 'QuerySet':
			result.append(question)
		elif return_type == 'dict':
			result.append(question.convertToDict())

	return result

def getMaxLevel(value):

	level = -1
	values = settings.GAME_SETTING['QUESTION']['EntryValue']
	for val in values:
		if value < val: 
			return level
		level += 1

	return level

if settings.HTML_TEST:

	def test_randomly(request):
		try:
			# 获取数据
			data = processRequest(request, POST=['count'])

			convertRequestDataTypeAll(data, 'int')

			count = data['count']

			pushRandomlyQuestions(count)

		except ErrorException as exception:
			return getErrorResponse(exception)

		# 返回成功信息
		return getSuccessResponse()

	def test_delete(request):
		try:
			# 获取数据
			data = processRequest(request)

			deleteTestQuestions()

		except ErrorException as exception:
			return getErrorResponse(exception)

		# 返回成功信息
		return getSuccessResponse()

def pushRandomlyQuestions(count):
	scnt = 9
	lcnt = 6
	qid = 0
	for i in range(count):
		for s in range(scnt):
			for l in range(lcnt):
				qid += 1
				qtype = random.randint(1,4)
				ccnt = random.randint(3,6)
				crtid = -1
				if qtype != 2:
					crtid = random.randint(0,ccnt)
				choices = []
				question = Question()
				question.title = '测试'+str(qid)
				question.level = l
				question.subject = Subject.objects.get(id=s+1)
				question.type = QuestionType.objects.get(id=qtype)
				question.for_test = True
				question.save()

				for c in range(ccnt):
					answer = c == crtid
					if qtype == 2:
						answer = random.randint(0,1)==1
					choice = Choice()
					if answer:
						choice.text = '正确'
						choice.answer = True
					else:
						choice.text = '错误'
					choice.question = question
					choice.save()
					"""
					choices.append(choice)


				for choice in choices:
					choice.save()"""

def deleteTestQuestions():

	questions = Question.objects.filter(for_test = True)

	for question in questions:
		question.delete()