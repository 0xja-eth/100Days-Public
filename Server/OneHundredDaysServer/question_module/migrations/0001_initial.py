# Generated by Django 2.2 on 2019-04-27 19:06

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    initial = True

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='QuestionType',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('text', models.CharField(max_length=4)),
            ],
        ),
        migrations.CreateModel(
            name='Subject',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=4)),
            ],
        ),
        migrations.CreateModel(
            name='Question',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('title', models.TextField()),
                ('description', models.TextField()),
                ('level', models.PositiveSmallIntegerField()),
                ('score', models.PositiveSmallIntegerField(default=6)),
                ('subjectId', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='question_module.Subject')),
                ('typeId', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='question_module.QuestionType')),
            ],
        ),
        migrations.CreateModel(
            name='Choice',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('text', models.TextField()),
                ('answer', models.BooleanField()),
                ('questionID', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='question_module.Question')),
            ],
        ),
    ]
