from __future__ import absolute_import, unicode_literals
from celery import shared_task
import requests
from bs4 import BeautifulSoup
from datetime import datetime
from .models import Channel, News
import re
import json


@shared_task
def crawNews():
    channels = Channel.objects.all()

    output = ''
    for chan in channels:
        # get last id of news
        lastid = chan.lastid

        # try to get latest news
        url = chan.link
        headers = {'User-Agent':'Mozilla/5.0 (Windows 10.0; Win64; x64) AppleWebKit/537.36'}
        r = requests.get(url, headers=headers)
        res = r.text
        dic = json.loads(res)
        tab_list = dic['tab_list']
        if lastid == tab_list[0]['postid']:
            return 

        for tab in tab_list:
            title = tab['title']
            postid = tab['postid']
            imgsrc = tab['imgsrc']
            
            # compare with the last id
            # if == id, then stop
            if lastid == postid:
                break
            crawDetail(postid, imgsrc, title)
            output = output + ' ' + title + postid +  '<br>'
        chan.lastid = tab_list[0]['postid']
        chan.save()


def crawDetail(postid, imgsrc, title):
    url = 'https://c.m.163.com/news/a/'+postid+'.html'
    headers = {'User-Agent':'Mozilla/5.0 (Windows 10.0; Win64; x64) AppleWebKit/537.36'}
    r = requests.get(url, headers=headers)
    res = r.text
    ## find all gifs
    urls = re.findall(r'"src":"([^"]*)', res)

    n = News(title=title, imglink=imgsrc, content = ' '.join(urls))
    n.save()