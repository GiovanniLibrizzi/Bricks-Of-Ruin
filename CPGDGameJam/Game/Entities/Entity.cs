using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class Entity {
        public int ID { get; set; }
        public List<Component> components = new List<Component>();
        public World world;

        public Entity (World world) {
            this.world = world;
        }

        public void AddComponent(Component component) {
            components.Add(component);
            component.entity = this;
        }

        public virtual void Update(GameTime gameTime) { }


        public T GetComponent<T>() where T : Component {
            foreach (Component component in components) {
                if (component.GetType().Equals(typeof(T))) {
                    return (T)component;
                }
            }
            return null;
        }

        public bool HasComponent<T>() where T : Component {
            foreach (Component component in components) {
                if (component.GetType().Equals(typeof(T))) {
                    return true;
                }
            }
            return false;
        }
    }
}
