using System;
using TinyMVVM;

namespace LogWork.Models
{
    public class MenuItem : TinyModel
    {
        private int id;
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        private string detail;
        public string Detail { get => detail; set => SetProperty(ref detail, value); }

        private string icon;
        public string Icon { get => icon; set => SetProperty(ref icon, value); }

        private Type targetType;
        public Type TargetType { get => targetType; set => SetProperty(ref targetType, value); }
    }
}