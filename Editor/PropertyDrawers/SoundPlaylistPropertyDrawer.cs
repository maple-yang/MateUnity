﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace M8 {
    [CustomPropertyDrawer(typeof(SoundPlaylistAttribute))]
    public class SoundPlaylistPropertyDrawer : PropertyDrawer {
        private static SoundPlaylist mPlaylist;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(!mPlaylist) {
                //manually grab
                var objects = Resources.FindObjectsOfTypeAll<SoundPlaylist>();
                if(objects.Length > 0) {
                    mPlaylist = objects[0];
                }
            }

            if(mPlaylist) {
                EditorGUI.BeginProperty(position, label, property);

                //generate names
                var soundNameList = new List<string>();
                soundNameList.Add("<None>");
                for(int i = 0; i < mPlaylist.sounds.Length; i++)
                    soundNameList.Add(mPlaylist.sounds[i].name);

                var curMusicName = property.stringValue;

                //get current take name list index
                int index = -1;
                if(string.IsNullOrEmpty(curMusicName)) {
                    index = 0;
                }
                else {
                    for(int i = 1; i < soundNameList.Count; i++) {
                        if(soundNameList[i] == curMusicName) {
                            index = i;
                            break;
                        }
                    }
                }

                //select
                index = EditorGUI.Popup(position, label.text, index, soundNameList.ToArray());
                if(index >= 1 && index < soundNameList.Count)
                    property.stringValue = soundNameList[index];
                else
                    property.stringValue = "";

                EditorGUI.EndProperty();
            }
            else
                EditorGUI.PropertyField(position, property, label);
        }
    }
}