using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace Spline
{
    [CustomEditor(typeof(SplineCloner))]
    public class SplineClonerEditor : Editor
    {
        private SplineCloner _splineCloner;
        private VisualElement _RootElement;
        private VisualTreeAsset _BeatMachineTemplate;

        public void OnEnable()
        {
            //Bindings on the UI elements will target the attached BeatMachine.cs script
            _splineCloner = (SplineCloner)target;
            _RootElement = new VisualElement();
            _RootElement.name = "RootElement";
            _BeatMachineTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SplineCloner/Scripts/Editor/SplineUI.uxml");
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SplineCloner/Scripts/Editor/SplineUI.uss");
            _RootElement.styleSheets.Add(stylesheet);

            _RootElement.Clear();


            //Clone the visual tree into our Visual Elements using the templates
            _BeatMachineTemplate.CloneTree(_RootElement);

            Button place = _RootElement.Query<Button>("PlaceObjects");
            if (place != null)
                place.clickable.clicked += () => _splineCloner.PlaceObjects();

            ObjectField objField = _RootElement.Query<ObjectField>("ObjectToClone");
            if (objField != null)
                objField.objectType = (typeof(GameObject));

            ObjectField controlPoints = _RootElement.Query<ObjectField>("ControlPoints");
            if (controlPoints != null)
                controlPoints.objectType = (typeof(Transform));

            var sliders = _RootElement.Query<Slider>().ToList();
            foreach (Slider s in sliders)
            {
                s.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    s.tooltip = s.value.ToString("F2");

                });
            }

            var sliderInts = _RootElement.Query<SliderInt>().ToList();
            foreach (SliderInt s in sliderInts)
            {
                s.RegisterCallback<ChangeEvent<int>>((evt) =>
                {
                    s.tooltip = s.value.ToString("F2");
                });
            }

            var minMax = _RootElement.Query<MinMaxSlider>().First();
            if (minMax != null)
            {
                minMax.RegisterCallback<ChangeEvent<Vector2>>((evt) =>
                {
                    minMax.tooltip = _splineCloner.m_scaleMinMax.x.ToString("F4") + ", " + _splineCloner.m_scaleMinMax.y.ToString("F4");
                });
            }

        }

        //Overwrite and build the Editor interface
        public override VisualElement CreateInspectorGUI()
        {

            return _RootElement;
        }


    }
}