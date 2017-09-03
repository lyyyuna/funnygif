from django.db import models
from django.utils import timezone

# Create your models here.
class Channel(models.Model):
    title = models.CharField(max_length=50, default='error')
    lasttime = models.DateTimeField(auto_now=True)
    link = models.URLField(default='error')
    lastid = models.CharField(max_length=20, default='error')


class News(models.Model):
    title = models.CharField(max_length=50, default='error')
    imglink = models.URLField(default='error')
    content = models.TextField(default='error')


